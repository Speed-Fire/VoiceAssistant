using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.Base
{
	public class PluginInfo(
		string name,
		string? description,
		string version,
		string? authors,
		string? urlAddress,
		Stream? imageStream = null)
	{
		public string Name { get; } = name;
		public string? Description { get; } = description;
		public string Version { get; } = version;
		public string? Authors { get; } = authors;
		public string? UrlAddress { get; } = urlAddress;
		public Stream? ImageStream { get; } = imageStream;
	}
}
