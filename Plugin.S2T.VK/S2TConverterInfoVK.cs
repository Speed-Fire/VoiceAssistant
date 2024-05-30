using Plugin.S2T.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.S2T.VK
{
	internal class S2TConverterInfoVK : S2TConverterInfo
	{
		public S2TConverterInfoVK(Func<S2TConverterVK> factory)
			: base("VK Converter", "Supports only Russian language.", factory) { }
	}
}
