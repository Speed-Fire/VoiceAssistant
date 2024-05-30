using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.S2T.VK.Models
{
	internal class UploadUrlInfo
	{
		public DateOnly Date { get; set; }
		public string Url { get; set; } = string.Empty;
	}
}
