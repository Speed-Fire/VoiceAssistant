using LiteDB;
using Plugin.S2T.VK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Core.Interfaces;
using VoiceAssistant.Core.Misc;

namespace Plugin.S2T.VK.Misc
{
	internal class SettingsLoader : ISettingsLoader
	{
		private readonly Provider<ConverterSettings> _settings;

		public SettingsLoader(Provider<ConverterSettings> settings)
		{
			_settings = settings;
		}

		public void Load()
		{
			using var db = new LiteDatabase("Data/S2T.VK.db");

			var col = db.GetCollection<ConverterSettings>();
			_settings.Value = col.Query().FirstOrDefault();
		}
	}
}
