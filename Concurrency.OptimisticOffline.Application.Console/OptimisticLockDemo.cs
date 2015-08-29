using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Concurrency.OptimisticOffline.Repository.Memory.Repositories;
using Concurrency.OptimisticOffline.Repository.Memory.UnitOfWork;

namespace Concurrency.OptimisticOffline.Application.Console
{
	public class OptimisticLockDemo
	{
		public OptimisticLockDemo()
		{
		}

		public void EditSameEntity()
		{
			var id = new Guid("a004c037-294d-4796-b51b-070a4e832241");
			var manager = SessionManager.GetManager();
			var session1 = manager.Open();
			var session2 = manager.Open();

			manager.Current = session1;
			var repository = new CustomerRepository(new UnitOfWork());

			var customerS1 = repository.FindBy(id);

			manager.Current = session2;
			var customerS2 = repository.FindBy(id);
		}
	}
}
