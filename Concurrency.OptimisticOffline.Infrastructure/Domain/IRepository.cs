using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Concurrency.OptimisticOffline.Infrastructure
{
	public interface IRepository<T> where T : IAggregateRoot
	{
		T FindBy(Guid id);
		IEnumerable<T> FindAll();

		void Save(T entity);
		void Add(T entity);
		void Remove(T entity);
	}
}
