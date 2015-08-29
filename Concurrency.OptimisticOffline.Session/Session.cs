using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Concurrency.OptimisticOffline.Infrastructure;

namespace Concurrency.OptimisticOffline.Application.Console
{
	public class Session : ISession
	{
		private IdentityMap map;

		public Session()
		{
			this.Id = Guid.NewGuid();
			this.map = new IdentityMap();
		}

		#region ISession implementation

		public Guid Id { get; private set; }

		public IdentityMap GetIdentityMap()
		{
			return this.map;
		}

		public void Close()
		{
			this.map.Clear();
		}

		#endregion
	}
}