using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Concurrency.OptimisticOffline.Infrastructure.Domain;
using Concurrency.OptimisticOffline.Repository.Sql.Data;
using Concurrency.OptimisticOffline.Session;

namespace Concurrency.OptimisticOffline.Repository.Sql.Mapper
{
	internal static class SqlMapperHelper
	{
		public static string GetSqlSyntax(this Guid value)
		{
			return string.Format("'{0}'", value);
		}

		public static string GetSqlSyntax(this int value)
		{
			return GetSqlSyntax(value.ToString());
		}

		public static string GetSqlSyntax(this DateTime value)
		{
			return GetSqlSyntax(value.ToString());
		}

		public static string GetSqlSyntax(this string value)
		{
			return string.Format("'{0}'", value);
		}
	}

	internal static class CRUDBase
	{
		private static readonly string INSERT = "INSERT INTO {0} VALUES({1})";
		private static readonly string LOAD = "SELECT {0} FROM {1} WHERE {2} = {3}";
		private static readonly string UPDATE = "UPDATE {0} SET {1} WHERE {2}";
		private static readonly string DELETE = "DELETE FROM {0} WHERE {1} = {2} AND {3} = {4}";

		public static string GetLoadQuery(ITableDefinition table, Func<string> GetIdValue)
		{
			var columns = GetColumns(table);
			return string.Format(CRUDBase.LOAD, columns, table.Name, SystemColumns.Id, GetIdValue());
		}

		public static string GetDeleteQuery(ITableDefinition table, Func<string, string> GetContidionValue)
		{
			return string.Format(DELETE, table.Name, SystemColumns.Id, GetContidionValue(SystemColumns.Id), SystemColumns.Version, GetContidionValue(SystemColumns.Version));
		}

		public static string GetUpdateQuery(ITableDefinition table, Func<IColumnDefinition, bool> CanUpdate, Func<IColumnDefinition, bool> IsWhere, Func<IColumnDefinition, bool, string> GetValue)
		{
			var id = table.SystemColumns[SystemColumns.Id];
			var version = table.SystemColumns[SystemColumns.Version];

			var sets = new Dictionary<string, string>();
			var whereSet = new Dictionary<string, string>();
			int index = 0;

			foreach (var column in table.Columns)
			{
				if (CanUpdate(column))
					sets.Add(column.Name, GetValue(column, false));
				if (IsWhere(column))
					whereSet.Add(column.Name, GetValue(column, true));
			}

			var setInfo = new StringBuilder();
			int s = 0;
			foreach(var set in sets)
			{
				if (s < sets.Count - 1)
					setInfo.Append(string.Format("{0} = {1}, ", set.Key, set.Value));
				else
					setInfo.Append(string.Format("{0} = {1}", set.Key, set.Value));
				s++;
			}

			var whereInfo = new StringBuilder();
			int w = 0;
			foreach(var where in whereSet)
			{
				if (w < whereSet.Count - 1)
					whereInfo.Append(string.Format("{0} = {1} AND ", where.Key, where.Value));
				else
					whereInfo.Append(string.Format("{0} = {1}", where.Key, where.Value));
				w++;
			}

			return string.Format(UPDATE, table.Name, setInfo.ToString(), whereInfo.ToString());
		}

		private static string GetColumns(ITableDefinition table)
		{
			var columns = new StringBuilder();
			for (int i = 0; i < table.Columns.Length; i++)
			{
				columns.Append(table.Columns[i].Name);
				if (i < table.Columns.Length - 1)
					columns.Append(", ");
			}
			return columns.ToString();
		}
	}

	public abstract class BaseMapper<T> where T : EntityBase
	{
		private ITableDefinition table;

		public BaseMapper(ITableDefinition table)
		{
			this.table = table;
		}

		public T Find(Guid id)
		{
			var manager = SessionManager.Manager;
			var session = manager.GetSession(manager.Current);
			var entity = (T)session.GetIdentityMap().Get(id);
			if (entity == null)
			{
				SqlConnection conn = null;
				try
				{
					conn = (SqlConnection)session.DbInfo.Connection;
					var command = conn.CreateCommand();
					command.CommandType = CommandType.Text;
					command.CommandText = CRUDBase.GetLoadQuery(table, () => id.GetSqlSyntax());
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						entity = Load(id, reader);
						string modifiedBy = reader[SystemColumns.ModifiedBy].ToString();
						DateTime modified = DateTime.Parse(reader[SystemColumns.Modified].ToString());
						int version = int.Parse(reader[SystemColumns.Version].ToString());
						entity.SetSystemFields(modified, modifiedBy, version);
						session.GetIdentityMap().Add(id, entity);
					}
					else
					{
						throw new Exception(table.Name + " " + id.ToString() + "does not exist");
					}
				}
				catch (SqlException sqlEx)
				{
					throw new Exception("unexpected error finding " + table.Name + " " + id);
				}
			}
			return entity;
		}

		protected abstract T Load(Guid id, SqlDataReader reader);

		public void Insert(T entity)
		{
			throw new NotImplementedException();
		}

		public void Delete(T entity)
		{
			var manager = SessionManager.Manager;
			var session = manager.GetSession(manager.Current);
			session.GetIdentityMap().Remove(entity.Id);
			SqlConnection conn = null;
			var query = string.Empty;
			try
			{
				conn = (SqlConnection)session.DbInfo.Connection;
				using (var command = conn.CreateCommand())
				{
					command.CommandType = CommandType.Text;
					command.CommandText = CRUDBase.GetDeleteQuery(table, (f) =>
					{
						if (f == SystemColumns.Id)
							return entity.Id.GetSqlSyntax();
						else if (f == SystemColumns.Version)
							return entity.Version.GetSqlSyntax();
						throw new NotSupportedException();
					});
					command.Transaction = (SqlTransaction)session.DbInfo.Transaction;
					var records = command.ExecuteNonQuery();
					if (records == 0)
						ThrowConcurrencyException(entity);
				}
			}
			catch (SqlException e)
			{
				throw new Exception("unexpected error deleting");
			}
		}

		protected void ThrowConcurrencyException(T entity)
		{
			throw new Exception("Concurrency Exception on " + entity.Id);
		}

		public void Update(T entity)
		{
			var manager = SessionManager.Manager;
			var session = manager.GetSession(manager.Current);
			SqlConnection conn = null;
			try
			{
				int oldVersion = entity.Version;
				entity.SetSystemFields(DateTime.UtcNow, entity.ModifiedBy, entity.Version + 1);
				conn = (SqlConnection)session.DbInfo.Connection;
				using (var command = conn.CreateCommand())
				{
					command.CommandType = CommandType.Text;
					command.CommandText = CRUDBase.GetUpdateQuery(table, CanUpdate, IsWhere, 
						(cd, isWhere) =>
							{
								if (cd.Name == SystemColumns.Version && isWhere)
									return oldVersion.GetSqlSyntax();
								return GetValue(entity, cd);
							});
					command.Transaction = (SqlTransaction)session.DbInfo.Transaction;
					var rows = command.ExecuteNonQuery();
					if (rows == 0)
						ThrowConcurrencyException(entity);
				}
			}
			catch (SqlException e)
			{
				throw new Exception("unexpected error updating");
			}
		}

		private bool CanUpdate(IColumnDefinition definition)
		{
			var name = definition.Name;
			return (name != SystemColumns.Id && name != SystemColumns.Created && name != SystemColumns.CreatedBy);
		}

		private bool IsWhere(IColumnDefinition definition)
		{
			var name = definition.Name;
			return (name == SystemColumns.Id || name == SystemColumns.Version);
		}

		private string GetValue(T entity, IColumnDefinition definition)
		{
			var value = GetSystemValue(entity, definition);
			if (value == null)
				value = GetEntityValue(entity, definition);
			return value;
		}

		private string GetSystemValue(T entity, IColumnDefinition definition)
		{
			if (definition.Name == SystemColumns.Id) return entity.Id.GetSqlSyntax();
			else if (definition.Name == SystemColumns.Created) return entity.Created.GetSqlSyntax();
			else if (definition.Name == SystemColumns.CreatedBy) return entity.CreatedBy.GetSqlSyntax();
			else if (definition.Name == SystemColumns.Modified) return entity.Modified.GetSqlSyntax();
			else if (definition.Name == SystemColumns.ModifiedBy) return entity.ModifiedBy.GetSqlSyntax();
			else if (definition.Name == SystemColumns.Version) return entity.Version.GetSqlSyntax();
			return null;
		}

		protected abstract string GetEntityValue(T entity, IColumnDefinition definition, bool forWhere = false);
	}
}
