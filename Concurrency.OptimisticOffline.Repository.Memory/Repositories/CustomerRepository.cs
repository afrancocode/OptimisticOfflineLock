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
	public sealed class CustomerRepository : ICustomerRepository, IUnitOfWorkRepository
	{
		private IUnitOfWork uow;
		private CustomerMapper mapper;

		public CustomerRepository(IUnitOfWork uow)
		{
			this.uow = uow;
			this.mapper = new CustomerMapper();
		}

		public Customer FindBy(string name)
		{
			Customer customer = null;
			foreach(var item in mapper.FindAll())
			{
				customer = (Customer)item;
				if (customer.Name == name)
					return customer;
			}
			return customer;
		}

		public Customer FindBy(Guid id)
		{
			return (Customer)mapper.Find(id);
		}

		public IEnumerable<Customer> FindAll()
		{
			foreach(var item in mapper.FindAll())
			{
				yield return (Customer)item;
			}
		}

		public void Save(Customer entity)
		{
			this.uow.RegisterAmended(entity, this);
		}

		public void Add(Customer entity)
		{
			this.uow.RegisterNew(entity, this);
		}

		public void Remove(Customer entity)
		{
			this.uow.RegisterRemoved(entity, this);
		}

		void IUnitOfWorkRepository.PersistCreationOf(IAggregateRoot entity)
		{
			
		}

		void IUnitOfWorkRepository.PersistUpdateOf(IAggregateRoot entity)
		{
			throw new NotImplementedException();
		}

		void IUnitOfWorkRepository.PersistDeletionOf(IAggregateRoot entity)
		{
			throw new NotImplementedException();
		}
	}
}
