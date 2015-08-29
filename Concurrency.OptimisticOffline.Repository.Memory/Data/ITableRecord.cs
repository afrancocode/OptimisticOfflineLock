using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Concurrency.OptimisticOffline.Repository.Memory.Data
{
	public interface ITableRecord
	{
		Guid Id { get; set; }
		string ModifiedBy { get; set; }
		DateTime Modified { get; set; }
		int Version { get; set; }
	}
}
