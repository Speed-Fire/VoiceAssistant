using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAssistant.Misc.FilteringCollection
{
	public interface ICollectionFilter<TItem>
	{
		bool Filter(TItem item);
	}

	public class FuncCollectionFilter<T>(Func<T, bool> filter) : ICollectionFilter<T>
	{
		private readonly Func<T, bool> _filter = filter;

		public bool Filter(T item)
		{
			return _filter.Invoke(item);
		}
	}
}
