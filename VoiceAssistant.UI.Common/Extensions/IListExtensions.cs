using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAssistant.UI.Common.Extensions
{
	public static class IListExtensions
	{
		public static int IndexOf<T>(this IList<T> list, Func<T, bool> predicate)
		{
			for(int i = 0; i < list.Count; i++)
			{
				if (predicate(list[i]))
					return i;
			}

			return -1;
		}
	}
}
