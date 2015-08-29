using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Concurrency.OptimisticOffline.Application.Console;
using Concurrency.OptimisticOffline.Infrastructure.Domain;
using Concurrency.OptimisticOffline.Repository.Memory.Data;

namespace Concurrency.OptimisticOffline.Repository.Memory.Mapper
{
	public abstract class BaseMapper
	{
		public BaseMapper() { }

		public EntityBase Find(Guid id)
		{
			var manager = SessionManager.GetManager();
			var obj = manager.GetSession(manager.Current).GetIdentityMap().Get(id);
			if (obj == null)
			{
				var record = ExecuteLoadQuery(id);
				if (record != null)
				{
					obj = Load(id, record);
					obj.SetSystemFields(record.Modified, record.ModifiedBy, record.Version);
					manager.GetSession(manager.Current).GetIdentityMap().Add(id, obj);
				}
				else
				{
					throw new Exception("Record not found");
				}
			}
			return obj;
		}

		public IEnumerable<EntityBase> FindAll()
		{
			var manager = SessionManager.GetManager();
			var table = GetTable();
			foreach(var record in table.GetAll())
			{
				var obj = manager.GetSession(manager.Current).GetIdentityMap().Get(record.Id);
				if(obj == null)
				{
					obj = Load(record.Id, record);
					obj.SetSystemFields(record.Modified, record.ModifiedBy, record.Version);
					manager.GetSession(manager.Current).GetIdentityMap().Add(record.Id, obj);
				}
				yield return obj;
			}
		}

		private ITableRecord ExecuteLoadQuery(Guid id)
		{
			var table = GetTable();
			foreach (var record in table.GetAll())
			{
				if (id == record.Id)
					return record;
			}
			return null;
		}

		protected abstract EntityBase Load(Guid id, ITableRecord record);
		protected abstract ITable GetTable();

		public void Update(EntityBase entity)
		{
			var manager = SessionManager.GetManager();
			var table = GetTable();
			var record = Generate(entity);
			// TODO: Concurrency
			table.Update(record);
		}

		protected abstract ITableRecord Generate(EntityBase entity);
	}
}
