using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Concurrency.OptimisticOffline.Infrastructure
{
	public interface ISession
	{
		Guid Id { get; }
		IDbSessionInfo DbInfo { get; }
		IdentityMap GetIdentityMap ();
		void Close();
	}

	public interface IDbSessionInfo
	{
		IDbConnection Connection { get; }
		IDbTransaction Transaction { get; set; }
	}
}
