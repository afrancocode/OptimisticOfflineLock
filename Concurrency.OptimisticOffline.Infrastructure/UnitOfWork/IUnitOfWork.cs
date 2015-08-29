using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Concurrency.OptimisticOffline.Infrastructure.UnitOfWork
{
	public interface IUnitOfWork
	{
		void RegisterAmended(IAggregateRoot entity, IUnitOfWorkRepository repository);
		void RegisterNew(IAggregateRoot entity, IUnitOfWorkRepository repository);
		void RegisterRemoved(IAggregateRoot entity, IUnitOfWorkRepository repository);
		void Commit();
	}
}
