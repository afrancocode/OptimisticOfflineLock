using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Concurrency.OptimisticOffline.Infrastructure;

namespace Concurrency.OptimisticOffline.Repository.Memory.Repositories
{
	public abstract class BaseRepository<T> : IRepository<T> where T : IAggregateRoot
	{
		public T FindBy(Guid id)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<T> FindAll()
		{
			throw new NotImplementedException();
		}

		public void Save(T entity)
		{
			throw new NotImplementedException();
		}

		public void Add(T entity)
		{
			throw new NotImplementedException();
		}

		public void Remove(T entity)
		{
			throw new NotImplementedException();
		}
	}
}
