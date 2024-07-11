using CommunityToolkit.Mvvm.ComponentModel;
using Synergy.WPF.Navigation.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Core.Interfaces;
using VoiceAssistant.Core.SettingsHelpers;
using VoiceAssistant.Services;
using VoiceAssistant.Views.Settings;

namespace VoiceAssistant.ViewModels.Settings
{
	internal partial class SpeechSynthesisSettingsVM : ViewModel<SpeechSynthesisSettingsView>
	{
		private readonly IUrgentNotifier _urgentNotifier;
		private readonly IApplicationSettingsService _settings;

		public IEnumerable<string> AvailableVoices { get; }

		[ObservableProperty]
		private string? _selectedVoice;

		[ObservableProperty]
		private int _volume;

        public SpeechSynthesisSettingsVM(
			IApplicationSettingsService settings,
			IUrgentNotifier urgentNotifier,
			IAssistantVoiceSettingsHelper assistantVoiceSettings)
        {
            _settings = settings;
			_urgentNotifier = urgentNotifier;

			AvailableVoices = assistantVoiceSettings.AvailableVoices;
			_selectedVoice = AvailableVoices
				.FirstOrDefault(v => v == assistantVoiceSettings.SelectedVoice);

			_volume = assistantVoiceSettings.Volume;

			_settings.SetCurrentSection("AssistantVoice");
        }

		async partial void OnSelectedVoiceChanged(string? value)
		{
			var ex = await _settings.SetValueAsync("SelectedVoice", value ?? string.Empty);
			if(ex is not null)
			{
				_urgentNotifier.NotifyError("Can't set synthesizer voice!",
					exception: ex);
			}
		}

		async partial void OnVolumeChanged(int value)
		{
			var tmp = int.Max(0, int.Min(100, value));

			var ex = await _settings.SetValueAsync("Volume", tmp.ToString());
			if (ex is not null)
			{
				_urgentNotifier.NotifyError("Can't set synthesizer voice!",
					exception: ex);
			}
		}
	}
}
