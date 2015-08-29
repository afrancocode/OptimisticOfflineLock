using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Concurrency.OptimisticOffline.Infrastructure
{
	public interface ISessionManager
	{
		Guid Current { get; set; }

		Guid Open();
		ISession GetSession(Guid sessionId);
		void Close(Guid sessionId);
	}
}
