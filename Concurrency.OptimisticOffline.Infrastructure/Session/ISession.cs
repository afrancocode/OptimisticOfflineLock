using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Concurrency.OptimisticOffline.Infrastructure
{
	public interface ISession
	{
		Guid Id { get; }
		IdentityMap GetIdentityMap ();
		void Close();
	}
}
