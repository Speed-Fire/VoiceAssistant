using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using Synergy.WPF.Navigation.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Core.Interfaces;
using VoiceAssistant.Services;

namespace VoiceAssistant.ViewModels.Settings.VoiceRecognition
{
	internal partial class GeminiSettingsVM : ViewModel
	{
		private readonly IApplicationSettingsService _settings;
		private readonly IUrgentNotifier _notifier;

		[ObservableProperty]
		private string _serviceApiKey;

		public GeminiSettingsVM(
			IApplicationSettingsService settings,
			IUrgentNotifier notifier,
			IConfiguration config)
		{
			_settings = settings;
			_notifier = notifier;

			_settings.SetCurrentSection("ChatGPT:Gemini");

			_serviceApiKey = config.GetRequiredSection("Application:ChatGPT:Gemini:ServiceApiKey").Value
				?? string.Empty;
		}

		[RelayCommand]
		private async Task Apply()
		{
			var result = await _settings.SetValueAsync("ServiceApiKey", ServiceApiKey);
			if(result is not null)
			{
				_notifier.NotifyError("Cannot set Gemini service API key!", exception: result);
			}
			else
			{
				var message = GetAppResource<string>("Strings.Settings.VoiceRecognition.Gemini.ServiceApiKey.Warning");

				_notifier.NotifyWarning(message);
			}
		}
	}
}
