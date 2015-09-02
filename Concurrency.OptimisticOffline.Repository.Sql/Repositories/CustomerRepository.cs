using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Concurrency.OptimisticOffline.Infrastructure.UnitOfWork;
using Concurrency.OptimisticOffline.Model.Model;
using Concurrency.OptimisticOffline.Repository.Sql.Mapper;

namespace Concurrency.OptimisticOffline.Repository.Sql.Repositories
{
	public sealed class CustomerRepository : Repository<Customer>, ICustomerRepository
	{
		public CustomerRepository(IUnitOfWork uow)
			: base(uow)
		{ }

		protected override BaseMapper<Customer> CreateMapper()
		{
			return new CustomerMapper();
		}
	}
}
