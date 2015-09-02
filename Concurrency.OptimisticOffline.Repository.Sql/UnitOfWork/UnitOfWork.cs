using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Concurrency.OptimisticOffline.Infrastructure;
using Concurrency.OptimisticOffline.Infrastructure.UnitOfWork;
using Concurrency.OptimisticOffline.Session;

namespace Concurrency.OptimisticOffline.Repository.Sql.UnitOfWork
{
	public class UnitOfWork : IUnitOfWork
	{
		private Dictionary<IUnitOfWorkRepository, List<IAggregateRoot>> add;
		private Dictionary<IUnitOfWorkRepository, List<IAggregateRoot>> update;
		private Dictionary<IUnitOfWorkRepository, List<IAggregateRoot>> remove;

		public UnitOfWork()
		{
			this.add = new Dictionary<IUnitOfWorkRepository, List<IAggregateRoot>>();
			this.update = new Dictionary<IUnitOfWorkRepository, List<IAggregateRoot>>();
			this.remove = new Dictionary<IUnitOfWorkRepository, List<IAggregateRoot>>();
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
			if (!add.TryGetValue(repository, out items))
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
			var manager = SessionManager.Manager;
			var session = manager.GetSession(manager.Current);

			var transaction = session.DbInfo.Connection.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);
			session.DbInfo.Transaction = transaction;
			try
			{
				InsertNew();
				DeleteRemoved();
				UpdateDirty();
				transaction.Commit();
			}
			catch
			{
				transaction.Rollback();
				throw;
			}
		}

		private void InsertNew() { }

		private void DeleteRemoved() { }

		private void UpdateDirty()
		{
			foreach(var updateInfo in this.update)
			{
				var persistTo = updateInfo.Key;
				while(updateInfo.Value.Any())
				{
					var entity = updateInfo.Value[0];
					persistTo.PersistUpdateOf(entity);
					updateInfo.Value.RemoveAt(0);
				}
			}
		}

		public static readonly IUnitOfWork Empty = new EmptyUnitOfWork();

		private sealed class EmptyUnitOfWork : IUnitOfWork
		{
			void IUnitOfWork.RegisterAmended(IAggregateRoot entity, IUnitOfWorkRepository repository) { }
			void IUnitOfWork.RegisterNew(IAggregateRoot entity, IUnitOfWorkRepository repository) { }
			void IUnitOfWork.RegisterRemoved(IAggregateRoot entity, IUnitOfWorkRepository repository) { }
			void IUnitOfWork.Commit() { }
		}
	}
}
