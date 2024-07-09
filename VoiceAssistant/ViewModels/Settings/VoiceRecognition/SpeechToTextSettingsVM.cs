using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Options;
using Plugin.S2T.Base;
using Synergy.WPF.Navigation.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Core.Interfaces;
using VoiceAssistant.Services;
using VoiceAssistant.Services.Options;

namespace VoiceAssistant.ViewModels.Settings.VoiceRecognition
{
	internal partial class SpeechToTextSettingsVM : ViewModel
	{
		private readonly ApplicationSettingsService _settings;
		private readonly IUrgentNotifier _notifier;

		public IEnumerable<string> AvailableConverters { get; }

		[ObservableProperty]
		private string? _selectedConverter;

		public SpeechToTextSettingsVM(
			ApplicationSettingsService settings,
			IUrgentNotifier notifier,
			IEnumerable<S2TConverterInfo> converterInfos,
			IOptions<SpeechToTextOptions> options)
		{
			_settings = settings;
			AvailableConverters = converterInfos.Select(i => i.Name).ToList();

			_settings.SetCurrentSection("SpeechToText");
			_notifier = notifier;

			SelectedConverter = AvailableConverters
					.FirstOrDefault(conv => conv == options.Value.SelectedConverter);
		}

		async partial void OnSelectedConverterChanged(string? value)
		{
			var result = await _settings.SetValueAsync("SelectedConverter", value ?? string.Empty);
			if(result is not null)
			{
				_notifier.NotifyError("Can't update SpeechToText converter!", exception: result);
			}
		}
	}
}
