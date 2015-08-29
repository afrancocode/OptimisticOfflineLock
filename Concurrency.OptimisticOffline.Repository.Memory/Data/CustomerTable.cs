using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Concurrency.OptimisticOffline.Repository.Memory.Data
{
	public class CustomerTable : ITable
	{
		private static readonly CustomerTable table = new CustomerTable();

		public static CustomerTable Table { get { return table; } }

		private List<CustomerRecord> records;

		public CustomerTable()
		{
			this.records = new List<CustomerRecord>();
			Seed();
		}

		public IEnumerable<ITableRecord> GetAll()
		{
			return this.records;
		}

		public void Add(CustomerRecord record)
		{
			this.records.Add(record);
		}

		public int Update(ITableRecord record)
		{
			return 0;
		}

		public void Remove(Guid id)
		{

		}

		private void Seed()
		{
			this.records.Add(new CustomerRecord()
			{
				Id = new Guid("a004c037-294d-4796-b51b-070a4e832241"),
				Name = "Alfonso",
				Address = "123 Main St",
				CreatedBy = "sysadmin",
				Created = DateTime.UtcNow.AddDays(-2),
				ModifiedBy = "sa",
				Modified = DateTime.UtcNow.AddDays(-1),
				Version = 1
			});

			this.records.Add(new CustomerRecord()
			{
				Id = new Guid("2e610a6e-12bd-4413-8a10-d49a9bb70a9e"),
				Name = "Zaid",
				Address = "344 Second St",
				CreatedBy = "sysadmin",
				Created = DateTime.UtcNow.AddDays(-2),
				ModifiedBy = "sa",
				Modified = DateTime.UtcNow.AddDays(-1),
				Version = 1
			});
		}
	}
}
