using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.S2T.VK.Extensions
{
	internal static class StreamExtensions
	{
		public static byte[] GetBytes(this Stream stream)
		{
			using var ms = new MemoryStream();
			stream.CopyTo(ms);
			return ms.ToArray();
		}
	}
}
