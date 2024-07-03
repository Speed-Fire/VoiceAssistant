using Plugin.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace VoiceAssistant.Entities
{
	public class PluginInfoEntity : PluginInfo
	{
		public BitmapFrame? Image { get; }

		public PluginInfoEntity(PluginInfo info) :
			base(info.Name,
				info.Description,
				info.Version,
				info.Authors,
				info.UrlAddress)
		{
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
