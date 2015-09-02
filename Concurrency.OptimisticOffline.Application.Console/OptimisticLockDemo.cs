using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Concurrency.OptimisticOffline.Infrastructure;
using Concurrency.OptimisticOffline.Infrastructure.UnitOfWork;
using Concurrency.OptimisticOffline.Model.Model;
using Concurrency.OptimisticOffline.Repository.Sql.Repositories;
using Concurrency.OptimisticOffline.Repository.Sql.UnitOfWork;
using Concurrency.OptimisticOffline.Session;

namespace Concurrency.OptimisticOffline.Application.Console
{
	public sealed class TestInfo
	{
		public void Initialize(Guid entityId)
		{
			this.EntityId = entityId;
			this.SessionId = SessionManager.Manager.Open();
			this.uow = new UnitOfWork();
			this.repository = new CustomerRepository(this.uow);
		}

		public Guid SessionId;
		public Guid EntityId;
		public Customer Entity;
		public IUnitOfWork uow;
		public ICustomerRepository repository;

		public ISession GetSession()
		{
			return SessionManager.Manager.GetSession(SessionId);
		}

		public void LoadCustomer()
		{
			SetCurrentSession();
			var connection = GetSession().DbInfo.Connection;
			connection.Open();
			try
			{
				this.Entity = this.repository.FindBy(EntityId);
			}
			finally
			{
				connection.Close();
			}
		}

		public void SaveCustomer()
		{
			SetCurrentSession();
			var connection = GetSession().DbInfo.Connection;
			connection.Open();
			try
			{
				this.repository.Save(Entity);
				this.uow.Commit();
			}
			catch(Exception e)
			{
				System.Console.WriteLine(e.Message);
			}
			finally
			{
				connection.Close();
			}
		}

		private void SetCurrentSession()
		{
			SessionManager.Manager.Current = SessionId;
		}
	}

	public class OptimisticLockDemo
	{
		public OptimisticLockDemo() { }

		public void EditSameEntity()
		{
			var id = new Guid("a004c037-294d-4796-b51b-070a4e832241");
			var manager = SessionManager.Manager;

			var user1 = new TestInfo();
			user1.Initialize(id);

			var user2 = new TestInfo();
			user2.Initialize(id);

			var user3 = new TestInfo();
			user3.Initialize(id);

			user1.LoadCustomer();
			user2.LoadCustomer();

			user1.Entity.Name = "Alfonso U1";
			user2.Entity.Name = "Alfonso U2";

			user2.SaveCustomer();
			user3.LoadCustomer(); // <-- User 3 reads after user2 commits
			user1.SaveCustomer();

			user3.Entity.Name = "Alfonso U3 wins";
			user3.SaveCustomer();
		}
	}
}
