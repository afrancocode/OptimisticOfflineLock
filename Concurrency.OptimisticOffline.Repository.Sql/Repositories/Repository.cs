using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Concurrency.OptimisticOffline.Infrastructure;
using Concurrency.OptimisticOffline.Infrastructure.Domain;
using Concurrency.OptimisticOffline.Infrastructure.UnitOfWork;
using Concurrency.OptimisticOffline.Repository.Sql.Mapper;

namespace Concurrency.OptimisticOffline.Repository.Sql.Repositories
{
	public abstract class Repository<T> : IRepository<T>, IUnitOfWorkRepository where T : EntityBase, IAggregateRoot
	{
		private IUnitOfWork uow;
		private BaseMapper<T> mapper;

		public Repository(IUnitOfWork uow)
		{
			Debug.Assert(uow != null);
			this.uow = uow;
		}

		protected BaseMapper<T> Mapper
		{
			get
			{
				if (this.mapper == null)
					this.mapper = CreateMapper();
				return this.mapper;
			}
		}

		protected abstract BaseMapper<T> CreateMapper();

		public T FindBy(Guid id)
		{
			return Mapper.Find(id);
		}

		public IEnumerable<T> FindAll()
		{
			throw new NotImplementedException();
		}

		public void Save(T entity)
		{
			this.uow.RegisterAmended(entity, this);
		}

		public void Add(T entity)
		{
			this.uow.RegisterNew(entity, this);
		}

		public void Remove(T entity)
		{
			this.uow.RegisterRemoved(entity, this);
		}

		#region IUnitOfWorkRepository implementation

		void IUnitOfWorkRepository.PersistCreationOf(IAggregateRoot entity)
		{
			PersistCreation((T)entity);
		}

		void IUnitOfWorkRepository.PersistUpdateOf(IAggregateRoot entity)
		{
			PersistUpdate((T)entity);
		}

		void IUnitOfWorkRepository.PersistDeletionOf(IAggregateRoot entity)
		{
			PersistDeletion((T)entity);
		}

		#endregion

		protected virtual void PersistCreation(T entity)
		{
			Mapper.Insert(entity);
		}

		protected virtual void PersistUpdate(T entity)
		{
			Mapper.Update(entity);
		}

		protected virtual void PersistDeletion(T entity)
		{
			Mapper.Delete(entity);
		}
	}
}
