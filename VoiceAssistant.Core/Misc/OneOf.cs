using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable disable

namespace VoiceAssistant.Core.Misc
{
	public class OneOf<T1, T2>
	{
		public T1 First { get; }
		public T2 Second { get; }

		public bool IsFirst => First != null;

        public OneOf(T1 value)
        {
            First = value;
        }

		public OneOf(T2 value)
		{
			Second = value;
		}
	}
}
