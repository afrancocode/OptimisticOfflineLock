using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Concurrency.OptimisticOffline.Model.Model;
using Concurrency.OptimisticOffline.Repository.Sql.Data;

namespace Concurrency.OptimisticOffline.Repository.Sql.Mapper
{
	public sealed class CustomerMapper : BaseMapper<Customer>
	{
		public CustomerMapper()
			: base(new CustomerTable())
		{ }

		protected override Customer Load(Guid id, SqlDataReader reader)
		{
			var name = reader["Name"].ToString();
			string createdBy = reader[SystemColumns.CreatedBy].ToString();
			DateTime created = DateTime.Parse(reader[SystemColumns.Created].ToString());
			return Customer.Activate(id, name, createdBy, created);
		}

		protected override string GetEntityValue(Customer entity, IColumnDefinition definition, bool forWhere = false)
		{
			if (definition.Name == "Name")
				return entity.Name.GetSqlSyntax();
			throw new NotSupportedException();
		}

		private sealed class CustomerTable : EntityTableDefintion
		{
			public CustomerTable() : base() { }
			public override string Name { get { return "customer"; } }
			protected override string[] GetColumnNames() { return new string[] { "Name" }; }
		}
	}
}
