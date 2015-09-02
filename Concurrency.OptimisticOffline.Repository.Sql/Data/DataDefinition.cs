using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Concurrency.OptimisticOffline.Repository.Sql.Data
{
	public sealed class ColumnDefinition : IColumnDefinition
	{
		public ColumnDefinition(string name) { this.Name = name; }
		public string Name { get; private set; }
	}

	public sealed class SystemColumnDefinition : ISystemColumnDefinitions
	{
		private Dictionary<string, IColumnDefinition> sysColumns;

		public SystemColumnDefinition(string[] columns)
		{
			sysColumns = new Dictionary<string, IColumnDefinition>();
			BuildColumns(columns);
		}

		private void BuildColumns(string[] columns)
		{
			foreach (var name in columns)
			{
				sysColumns.Add(name, new ColumnDefinition(name));
			}
		}

		public IColumnDefinition this[string name]
		{
			get { return sysColumns[name]; }
		}
	}

	public abstract class TableDefinition : ITableDefinition
	{
		private bool loaded;

		public TableDefinition()
		{
			BuildColumns();
		}

		public abstract string Name { get; }

		public ISystemColumnDefinitions SystemColumns { get; private set; }

		public IColumnDefinition[] Columns { get; private set; }

		private void BuildColumns()
		{
			if (loaded) return;

			this.SystemColumns = new SystemColumnDefinition(GetSystemColumnNames());

			var columns = GetSystemColumnNames().Concat(GetColumnNames()).ToArray();

			this.Columns = new IColumnDefinition[columns.Length];

			for (int c = 0; c < columns.Length; c++)
			{
				this.Columns[c] = new ColumnDefinition(columns[c]);
			}

			loaded = true;
		}

		protected abstract string[] GetColumnNames();
		protected abstract string[] GetSystemColumnNames();
	}

	public abstract class EntityTableDefintion : TableDefinition
	{
		public EntityTableDefintion() : base() { }

		protected override string[] GetSystemColumnNames()
		{
			return new string[] { "Id", "CreatedBy", "Created", "ModifiedBy", "Modified", "Version" };
		}
	}
}
