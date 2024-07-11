using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Common;
using VoiceAssistant.Core.Interfaces;
using VoiceAssistant.SpeechSynthesis.Options;

namespace VoiceAssistant.SpeechSynthesis
{
	internal class AssistantVoice : IAssistantVoice
	{
		internal readonly SpeechSynthesizer _synthesizer;
		private readonly PhraseDictionary _phraseDictionary;
		private readonly IUrgentNotifier _urgentNotifier;
		private readonly IOptionsMonitor<AssistantVoiceOptions> _options;
		private readonly SemaphoreSlim _semaphore = new(1, 1);

		private volatile bool _initialized = false;
		private VoiceInfo? _selectedVoice;

        public AssistantVoice(
			IUrgentNotifier urgentNotifier,
			IOptionsMonitor<AssistantVoiceOptions> options)
        {
			_synthesizer = new();
			_synthesizer.SetOutputToDefaultAudioDevice();

			_phraseDictionary = new();

			_options = options;
			_urgentNotifier = urgentNotifier;

			_options.OnChange(OnOptionsChanged);
        }

		public Task Initialize()
		{
			if(_initialized)
				return Task.CompletedTask;

			_initialized = true;

			return ChangeCurrentOptions(_options.CurrentValue);
		}

        public async Task Speak(string key)
		{
			await _semaphore.WaitAsync();

			if (_selectedVoice is null || _phraseDictionary.Culture is null)
				return;

			var phrase = _phraseDictionary[key];

			await SpeakPlain(phrase);

			_semaphore.Release();
		}

		public async Task Speak(VoicableException exception)
		{
			var currentCultureName = _phraseDictionary.Culture?.Name ?? string.Empty;

			if(!exception.VoiceMessages.TryGetValue(currentCultureName, out var message))
			{
				_urgentNotifier.NotifyError($"Voice exception culture is not supported! See additional info for exception content.",
					duration: 0, exception: exception);
				return;
			}

			await Speak(message);
		}

		private async Task SpeakPlain(string phrase)
		{
			var prompt = _synthesizer.SpeakAsync(phrase);

			await Task.Run(() =>
			{
				while (!prompt.IsCompleted) { }
			});
		}

		private void OnOptionsChanged(AssistantVoiceOptions options)
		{
			_ = ChangeCurrentOptions(options);
		}

		private async Task ChangeCurrentOptions(AssistantVoiceOptions options)
		{
			await _semaphore.WaitAsync();

			await ChangeVoice(options.SelectedVoice);
			_synthesizer.Volume = options.Volume;

			_semaphore.Release();
		}

		private async Task ChangeVoice(string voiceName)
		{
			if (_selectedVoice is not null && _selectedVoice.Name == voiceName)
				return;

			var voice = _synthesizer.GetInstalledVoices()
				.FirstOrDefault(v => v.VoiceInfo.Name == voiceName);

			if (voice is null)
			{
				_urgentNotifier.NotifyError($"Voice \"{voiceName}\" is not found!");
				return;
			}

			if (!voice.Enabled)
			{
				_urgentNotifier.NotifyError($"Voice \"{voiceName}\" cannot be set!");
				return;
			}

			if (!_phraseDictionary.IsCultureSupported(voice.VoiceInfo.Culture))
			{
				_urgentNotifier.NotifyError($"Unsupported voice culture!");
				return;
			}

			await _phraseDictionary.SetCulture(voice.VoiceInfo.Culture);

			_selectedVoice = voice.VoiceInfo;
			_synthesizer.SelectVoice(voiceName);
		}

		public void Dispose()
		{
			_semaphore.Dispose();
			_synthesizer.Dispose();
		}
	}
}
