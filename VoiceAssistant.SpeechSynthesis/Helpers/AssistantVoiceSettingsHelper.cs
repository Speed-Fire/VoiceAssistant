using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Core.Interfaces;
using VoiceAssistant.Core.SettingsHelpers;
using VoiceAssistant.SpeechSynthesis.Options;

namespace VoiceAssistant.SpeechSynthesis.Helpers
{
	public class AssistantVoiceSettingsHelper : IAssistantVoiceSettingsHelper
	{
		public IEnumerable<string> AvailableVoices { get; }
		public string? SelectedVoice { get; }
		public int Volume { get; }

		public AssistantVoiceSettingsHelper(
			IAssistantVoice assistantVoice,
			IOptions<AssistantVoiceOptions> options)
        {
            var assvoice = assistantVoice as AssistantVoice;
            if (assvoice is null)
                throw new InvalidOperationException();

			AvailableVoices = assvoice._synthesizer
				.GetInstalledVoices()
				.Select(iv => iv.VoiceInfo.Name)
				.ToList();

			SelectedVoice = options.Value.SelectedVoice;
			Volume = options.Value.Volume;
		}
	}
}
