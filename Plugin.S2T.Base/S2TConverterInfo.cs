using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.S2T.Base
{
	public class S2TConverterInfo(string name, Func<IS2TConverter> converterFactory)
	{
		public string Name { get; } = name;
		public Func<IS2TConverter> ConverterFactory { get; } = converterFactory;

		public bool IsInfoOfConverter(IS2TConverter converter)
		{
			return ConverterFactory.Method.ReturnType == converter.GetType();
		}
	}
}
