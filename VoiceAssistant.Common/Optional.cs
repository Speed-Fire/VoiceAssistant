using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable disable

namespace VoiceAssistant.Common
{
	public class Optional<T>
	{
		public bool IsDefault { get; }
		public T Value { get; }

        public Optional()
        {
            IsDefault = true;
        }

        public Optional(T value)
        {
            Value = value;
            IsDefault = false;
        }

        public static implicit operator Optional<T>(T value) => new(value);
    }
}
