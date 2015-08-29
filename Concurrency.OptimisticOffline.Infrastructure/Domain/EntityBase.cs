using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Concurrency.OptimisticOffline.Infrastructure.Domain
{
	public abstract class EntityBase
	{
		private DateTime modified;
		private string modifiedBy;
		private int version;

		protected EntityBase() { }

		public Guid Id { get; set; }

		public DateTime Modified { get { return this.modified; } }
		public string ModifiedBy { get { return this.modifiedBy; } }
		public int Version { get { return this.version; } }

		public void SetSystemFields(DateTime modified, string modifiedBy, int version)
		{
			this.modified = modified;
			this.modifiedBy = modifiedBy;
			this.version = version;
		}

		public override bool Equals(object entity)
		{
			return entity != null
				&& entity is EntityBase
				&& this == (EntityBase)entity;
		}

		public override int GetHashCode()
		{
			return this.Id.GetHashCode();
		}

		public static bool operator ==(EntityBase entity1, EntityBase entity2)
		{
			if ((object)entity1 == null && (object)entity2 == null)
				return true;

			if ((object)entity1 == null || (object)entity2 == null)
				return false;

			if (entity1.Id.ToString() == entity2.Id.ToString())
				return true;

			return false;
		}

		public static bool operator !=(EntityBase entity1, EntityBase entity2)
		{
			return (!(entity1 == entity2));
		}
	}
}
