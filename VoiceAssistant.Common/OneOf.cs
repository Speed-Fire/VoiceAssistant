using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable disable

namespace VoiceAssistant.Common
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

		public static implicit operator OneOf<T1, T2>(T1 value) => new(value);
		public static implicit operator OneOf<T1, T2>(T2 value) => new(value);
	}
}
