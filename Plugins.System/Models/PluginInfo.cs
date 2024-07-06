using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginsSystem.Models
{
	public sealed class PluginInfo : IDisposable
	{
		public string Name { get; set; } = string.Empty;
		public string? Description { get; set; }
		public string Version { get; set; } = string.Empty;
		public string? Authors { get; set; }
		public string? UrlAddress { get; set; }
		public Stream? ImageStream { get; set; }
		public Stream? SettingsStream { get; set; }

		public string AssemblyName { get; set; } = string.Empty;

		public void Dispose()
		{
			ImageStream?.Dispose();
			SettingsStream?.Dispose();
		}
	}
}
