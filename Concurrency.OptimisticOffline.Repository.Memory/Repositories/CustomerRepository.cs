using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Concurrency.OptimisticOffline.Infrastructure;
using Concurrency.OptimisticOffline.Infrastructure.UnitOfWork;
using Concurrency.OptimisticOffline.Model.Model;
using Concurrency.OptimisticOffline.Repository.Memory.Mapper;

namespace Concurrency.OptimisticOffline.Repository.Memory.Repositories
{
	public sealed class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
	{
		private CustomerMapper mapper;

		public CustomerRepository(IUnitOfWork uow)
			: base(uow)
		{
			this.mapper = new CustomerMapper();
		}

		public Customer FindBy(string name)
		{
			foreach(var item in mapper.FindAll())
			{
				if (item.Name == name)
					return item;
			}
			return null;
		}

		public override Customer FindBy(Guid id)
		{
			return mapper.Find(id);
		}

		public override IEnumerable<Customer> FindAll()
		{
			return mapper.FindAll();
		}

		protected override void PersistCreation(Customer entity)
		{
			throw new NotImplementedException ();
		}

		protected override void PersistUpdate(Customer entity)
		{
			mapper.Update(entity);
		}

		protected override void PersistDeletion(Customer entity)
		{
			throw new NotImplementedException ();
		}
	}
}
