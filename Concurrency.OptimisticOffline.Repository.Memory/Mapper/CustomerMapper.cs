using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Concurrency.OptimisticOffline.Infrastructure.Domain;
using Concurrency.OptimisticOffline.Model.Model;
using Concurrency.OptimisticOffline.Repository.Memory.Data;

namespace Concurrency.OptimisticOffline.Repository.Memory.Mapper
{
	public sealed class CustomerMapper : BaseMapper<Customer, CustomerRecord>
	{
		private CustomerTable table;

		public CustomerMapper()
		{
			this.table = new CustomerTable();
		}

		protected override Customer Load(Guid id, CustomerRecord record)
		{
			return Customer.Activate(id, record.Name, record.Address);
		}

		protected override ITable GetTable()
		{
			return this.table;
		}

		protected override CustomerRecord Generate(Customer entity)
		{
			return new CustomerRecord()
			{
				Id = entity.Id,
				Name = entity.Name,
				Address = entity.Address,
				Created = entity.Created,
				CreatedBy = entity.CreatedBy,
				Modified = entity.Modified,
				ModifiedBy = entity.ModifiedBy
			};
		}
	}
}
