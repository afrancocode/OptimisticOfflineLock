using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Concurrency.OptimisticOffline.Repository.Memory.Data
{
	public abstract class Table<TRecord> : ITable where TRecord : TableRecord
	{
		void ITable.Add(ITableRecord record)
		{
			Add((TRecord)record);
		}

		int ITable.Update(ITableRecord record)
		{
			return Update(record.Id, (TRecord)record);
		}

		int ITable.Remove(ITableRecord record)
		{
			return Remove((TRecord)record);
		}

		IEnumerable<ITableRecord> ITable.GetAll()
		{
			return data;
		}

		private static readonly List<TRecord> data = new List<TRecord>();
		private static bool initialized = false;
		private static object _lock = new object();

		protected static void Seed(Func<IEnumerable<TRecord>> GetSeed)
		{
			lock(_lock)
			{
				if (initialized) return;
				foreach (var record in GetSeed())
					data.Add (record);
				initialized = true;
			}
		}

		public IEnumerable<TRecord> GetAll()
		{
			return data;
		}

		public void Add(TRecord record)
		{
			lock(_lock)
			{
				data.Add(record);
			}
		}

		public int Remove(TRecord record)
		{
			lock(_lock)
			{
				if (data.Remove(record))
					return 1;
				else
					return 0;
			}
		}

		public int Update(Guid id, TRecord record)
		{
			lock(_lock)
			{
				var target = data.Find(r => r.Id == id);
				if (target == null) return 0;
				UpdateSystem(record, target);
				Update(record, target);
				return 1;
			}
		}

		private void UpdateSystem(TRecord source, TRecord target)
		{
			target.Created = source.Created;
			target.CreatedBy = source.CreatedBy;
			target.Modified = source.Modified;
			target.ModifiedBy = source.ModifiedBy;
			target.Version = source.Version;
		}

		protected abstract void Update(TRecord source, TRecord target);
	}

	public class CustomerTable : Table<CustomerRecord>
	{
		public CustomerTable()
		{
			Seed(SeedProvider);
		}

		protected override void Update(CustomerRecord source, CustomerRecord target)
		{
			target.Created = source.Created;
			target.Name = source.Name;
			target.Address = source.Address;
		}

		private IEnumerable<CustomerRecord> SeedProvider()
		{
			yield return new CustomerRecord()
			{
				Id = new Guid("a004c037-294d-4796-b51b-070a4e832241"),
				Name = "Alfonso",
				Address = "123 Main St",
				CreatedBy = "sysadmin",
				Created = DateTime.UtcNow.AddDays(-2),
				ModifiedBy = "sa",
				Modified = DateTime.UtcNow.AddDays(-1),
				Version = 1
			};

			yield return new CustomerRecord()
			{
				Id = new Guid("2e610a6e-12bd-4413-8a10-d49a9bb70a9e"),
				Name = "Zaid",
				Address = "344 Second St",
				CreatedBy = "sysadmin",
				Created = DateTime.UtcNow.AddDays(-2),
				ModifiedBy = "sa",
				Modified = DateTime.UtcNow.AddDays(-1),
				Version = 1
			};
		}
	}
}
