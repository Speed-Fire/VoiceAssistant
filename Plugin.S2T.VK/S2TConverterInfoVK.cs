using Plugin.S2T.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.S2T.VK
{
	public class S2TConverterInfoVK(Func<IS2TConverter> factory) 
		:
		S2TConverterInfo("VK converter", factory)
	{
	}
}
