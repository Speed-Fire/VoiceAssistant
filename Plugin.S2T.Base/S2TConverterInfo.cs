using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.S2T.Base
{
	public class S2TConverterInfo(string name, string description, Func<IS2TConverter> converterFactory)
	{
		public string Name { get; } = name;
		public string Description { get; } = description;
		public Func<IS2TConverter> ConverterFactory { get; } = converterFactory;
	}
}
