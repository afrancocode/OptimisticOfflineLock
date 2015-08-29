using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Concurrency.OptimisticOffline.Infrastructure;
using Concurrency.OptimisticOffline.Infrastructure.UnitOfWork;

namespace Concurrency.OptimisticOffline.Repository.Memory.UnitOfWork
{
	public class UnitOfWork : IUnitOfWork
	{
		private Dictionary<IUnitOfWorkRepository, List<IAggregateRoot>> add;
		private Dictionary<IUnitOfWorkRepository, List<IAggregateRoot>> update;
		private Dictionary<IUnitOfWorkRepository, List<IAggregateRoot>> remove;

		public UnitOfWork()
		{
			add = new Dictionary<IUnitOfWorkRepository, List<IAggregateRoot>>();
			update = new Dictionary<IUnitOfWorkRepository, List<IAggregateRoot>>();
			remove = new Dictionary<IUnitOfWorkRepository, List<IAggregateRoot>>();
		}

		public void RegisterAmended(IAggregateRoot entity, IUnitOfWorkRepository repository)
		{
			List<IAggregateRoot> items = null;
			if (!update.TryGetValue(repository, out items))
			{
				items = new List<IAggregateRoot>();
				update.Add(repository, items);
			}
			items.Add(entity);
		}

		public void RegisterNew(IAggregateRoot entity, IUnitOfWorkRepository repository)
		{
			List<IAggregateRoot> items = null;
			if(!add.TryGetValue(repository, out items))
			{
				items = new List<IAggregateRoot>();
				add.Add(repository, items);
			}
			items.Add(entity);
		}

		public void RegisterRemoved(IAggregateRoot entity, IUnitOfWorkRepository repository)
		{
			List<IAggregateRoot> items = null;
			if (!remove.TryGetValue(repository, out items))
			{
				items = new List<IAggregateRoot>();
				remove.Add(repository, items);
			}
			items.Add(entity);
		}

		public void Commit()
		{
			throw new NotImplementedException();
		}
	}
}
