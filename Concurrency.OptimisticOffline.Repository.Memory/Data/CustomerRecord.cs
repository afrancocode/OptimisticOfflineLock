using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Concurrency.OptimisticOffline.Repository.Memory.Data
{
	public abstract class TableRecord : ITableRecord
	{
		public Guid Id { get; set; }
		public string ModifiedBy { get; set; }
		public DateTime Modified { get; set; }
		public int Version { get; set; }
	}

	/// <summary>
	/// Customer record.
	/// 	create table customer(id bigint primary key, name varchar, createdby varchar,
	///							  created datetime, modifiedby varchar, modified datetime, version int)
	/// </summary>
	public sealed class CustomerRecord : TableRecord
	{
		public CustomerRecord() { }

		public string Name { get; set; }
		public string Address { get; set; }
		public string CreatedBy { get; set; }
		public DateTime Created { get; set; }
	}
}
