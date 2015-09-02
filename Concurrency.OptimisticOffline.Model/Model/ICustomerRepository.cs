using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Concurrency.OptimisticOffline.Infrastructure.Domain;
using Concurrency.OptimisticOffline.Infrastructure;

namespace Concurrency.OptimisticOffline.Model.Model
{
	public interface ICustomerRepository : IRepository<Customer>
	{

	}
}
