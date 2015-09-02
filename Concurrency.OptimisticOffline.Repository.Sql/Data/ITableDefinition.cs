using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Concurrency.OptimisticOffline.Repository.Sql.Data
{
	public interface ITableDefinition
	{
		string Name { get; }
		ISystemColumnDefinitions SystemColumns { get; }
		IColumnDefinition[] Columns { get; }
	}
}
