using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartCashier.Infrastructure.DataVirtualization
{
	public class ItemsProvider<T> : IItemsProvider<T>
	{
		private readonly int _count;
		public Func<int, int, IList<T>> FetchRangeFunction { get; set; }

		public ItemsProvider(int count)
		{
			_count = count;
		}
		public virtual int FetchCount()
		{
			Trace.WriteLine("FetchCount");
			return _count;
		}
		public virtual IList<T> FetchRange(int startIndex, int count)
		{
			Trace.WriteLine("FetchRange: " + startIndex + "," + count);
			if (this.FetchRangeFunction != null)
			{
				return this.FetchRangeFunction(startIndex, count);
			}

			return null;
		}
	}

}
