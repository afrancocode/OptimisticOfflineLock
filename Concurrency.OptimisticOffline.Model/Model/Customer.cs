using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Concurrency.OptimisticOffline.Infrastructure;
using Concurrency.OptimisticOffline.Infrastructure.Domain;

namespace Concurrency.OptimisticOffline.Model.Model
{
	public sealed class Customer : EntityBase, IAggregateRoot
	{
		private Customer()
		{
		}

		public string Name { get; set; }

		public static Customer Activate(Guid id, string name, string createdBy, DateTime created)
		{
			return new Customer() { Id = id, Name = name, Created = created, CreatedBy = createdBy };
		}
	}
}
