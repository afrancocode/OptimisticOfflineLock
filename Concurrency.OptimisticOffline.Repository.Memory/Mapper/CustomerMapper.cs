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
	public sealed class CustomerMapper : BaseMapper
	{
		protected override EntityBase Load(Guid id, ITableRecord record)
		{
			var customerRecord = (CustomerRecord)record;
			return Customer.Activate(id, customerRecord.Name, customerRecord.Address);
		}

		protected override ITable GetTable()
		{
			return CustomerTable.Table;
		}

		protected override ITableRecord Generate(EntityBase entity)
		{
			throw new NotImplementedException();
		}
	}
}
