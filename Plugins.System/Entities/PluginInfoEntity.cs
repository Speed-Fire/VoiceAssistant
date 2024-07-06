using Plugin.Base;
using PluginsSystem.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using VoiceAssistant.Core.Models;

namespace PluginsSystem.Entities
{
    public class PluginInfoEntity
	{
		public string Name { get; }
		public string? Description { get; }
		public string Version { get; }
		public string? Authors { get; }
		public string? UrlAddress { get; }
		public string AssemblyName { get; }

		public BitmapFrame? Image { get; }
		public IReadOnlyList<ParameterInfoEntity> Parameters { get; }

		public PluginInfoEntity(PluginInfo info, IReadOnlyList<ParameterInfoEntity> parameters)
		{
			Name = info.Name;
			Description = info.Description;
			Version = info.Version;
			Authors = info.Authors;
			UrlAddress = info.UrlAddress;
			Parameters = parameters;
			AssemblyName = info.AssemblyName;
			
			if (info.ImageStream is not null)
				Image = CreateImage(info.ImageStream);

		}

		private static BitmapFrame CreateImage(Stream stream)
		{
			var bitmap = BitmapFrame.Create(stream, 
				BitmapCreateOptions.None, 
				BitmapCacheOption.OnLoad);

			stream.Close();
			stream.Dispose();

			return bitmap;
		}
	}
}
