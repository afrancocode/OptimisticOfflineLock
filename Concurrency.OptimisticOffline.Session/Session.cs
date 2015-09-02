using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Concurrency.OptimisticOffline.Infrastructure;

namespace Concurrency.OptimisticOffline.Session
{
	public class Session : ISession
	{
		private IdentityMap map;

		public Session(string connectionInfo)
		{
			this.Id = Guid.NewGuid();
			this.map = new IdentityMap();
			this.DbInfo = new DbSessionInfo(connectionInfo);
		}

		#region ISession implementation

		public Guid Id { get; private set; }

		public IDbSessionInfo DbInfo { get; private set; }

		public IdentityMap GetIdentityMap()
		{
			return this.map;
		}

		public void Close()
		{
			this.map.Clear();
		}

		#endregion

		private sealed class DbSessionInfo : IDbSessionInfo
		{
			private IDbTransaction transaction;

			public DbSessionInfo(string info)
			{
				this.Connection = new SqlConnection(info);
			}

			public IDbConnection Connection { get; private set; }

			public IDbTransaction Transaction
			{
				get { return this.transaction; }
				set
				{
					if (this.transaction != null && value != null)
						throw new InvalidOperationException();
					this.transaction = value;
				}
			}
		}
	}
}