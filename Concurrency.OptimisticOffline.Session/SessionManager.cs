using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Concurrency.OptimisticOffline.Infrastructure;
using System.Diagnostics;

namespace Concurrency.OptimisticOffline.Application.Console
{
	public sealed class SessionManager : ISessionManager
	{
		private static readonly ISessionManager manager = new SessionManager();

		public static ISessionManager GetManager()
		{
			return manager;
		}

		private Dictionary<Guid, ISession> sessions;

		private SessionManager()
		{
			this.sessions = new Dictionary<Guid, ISession>();
		}

		#region ISessionManager implementation

		public Guid Current { get; set; }

		public Guid Open()
		{
			var session = new Session();
			this.sessions.Add(session.Id, session);
			return session.Id;
		}

		public ISession GetSession(Guid sessionId)
		{
			Debug.Assert(this.sessions.ContainsKey(sessionId));
			return this.sessions[sessionId];
		}

		public void Close(Guid sessionId)
		{
			Debug.Assert(this.sessions.ContainsKey(sessionId));
			this.sessions[sessionId].Close();
			this.sessions.Remove(sessionId);
		}

		#endregion
	}
}
