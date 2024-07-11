using CommunityToolkit.Mvvm.Input;
using Synergy.WPF.Navigation.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Core.Interfaces;
using VoiceAssistant.Services;
using VoiceAssistant.ViewModels.Settings.VoiceRecognition;
using VoiceAssistant.Views.Settings;

namespace VoiceAssistant.ViewModels.Settings
{
	internal partial class VoiceRecognitionSettingsVM(
		SpeechToTextSettingsVM speech2TextSettings,
		GeminiSettingsVM geminiSettings)
		: ViewModel<VoiceRecognitionSettingsView>
	{
		public SpeechToTextSettingsVM SpeechToTextSettings { get; } = speech2TextSettings;
		public GeminiSettingsVM GeminiSettings { get; } = geminiSettings;
	}
}
