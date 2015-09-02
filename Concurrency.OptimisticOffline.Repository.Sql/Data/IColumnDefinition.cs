using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Concurrency.OptimisticOffline.Repository.Sql.Data
{
	public interface IColumnDefinition
	{
		string Name { get; }
	}

	public interface ISystemColumnDefinitions
	{
		IColumnDefinition this[string name] { get; }
	}
}
