using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Concurrency.OptimisticOffline.Infrastructure;
using Concurrency.OptimisticOffline.Infrastructure.UnitOfWork;

namespace Concurrency.OptimisticOffline.Repository.Memory.Repositories
{
	public abstract class BaseRepository<T> : IRepository<T>, IUnitOfWorkRepository where T : IAggregateRoot
	{
		private IUnitOfWork uow;

		public BaseRepository(IUnitOfWork uow)
		{
			this.uow = uow;
		}

		public abstract T FindBy(Guid id);
		public abstract IEnumerable<T> FindAll();

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

		protected abstract void PersistCreation(T entity);
		protected abstract void PersistUpdate(T entity);
		protected abstract void PersistDeletion(T entity);
	}
}
