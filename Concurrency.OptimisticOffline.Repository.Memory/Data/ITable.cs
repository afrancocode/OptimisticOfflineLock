﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Concurrency.OptimisticOffline.Repository.Memory.Data
{
	public interface ITable
	{
		IEnumerable<ITableRecord> GetAll();
		int Update(ITableRecord record);
	}
}