using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Core.Interfaces;

namespace VoiceAssistant.Services
{
	public class SettingsLoadingService
	{
		private IEnumerable<ISettingsLoader> _loaders;

		public SettingsLoadingService(IEnumerable<ISettingsLoader> loaders)
		{
			_loaders = loaders;
		}

		public Task LoadAsync()
		{
			return Task.Run(Load);
		}

		public void Load()
		{
			foreach (var loader in _loaders)
			{
				loader.Load();
			}
		}
	}
}
