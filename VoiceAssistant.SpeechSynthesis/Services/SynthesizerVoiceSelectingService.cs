using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Core.Interfaces;
using VoiceAssistant.SpeechSynthesis.Options;

namespace VoiceAssistant.SpeechSynthesis.Services
{
	public class SynthesizerVoiceSelectingService(
		IOptions<AssistantVoiceOptions> options,
		IServiceProvider serviceProvider,
		ILogger<SynthesizerVoiceSelectingService> logger) 
		: ISequentialInitializer
	{
		private readonly IOptions<AssistantVoiceOptions> _options = options;
		private readonly IServiceProvider _services = serviceProvider;
		private readonly ILogger _logger = logger;

		public async Task Initialize()
		{
			if (!string.IsNullOrWhiteSpace(_options.Value.SelectedVoice))
			{
				_logger.LogInformation("Synthesizer voice selection is skipped.");
				return;
			}

			_logger.LogInformation("Starting selecting synthesizer voice...");

			using var synthsezer = new SpeechSynthesizer();
			var voices = synthsezer
				.GetInstalledVoices()
				.Where(v => v.Enabled)
				.Where(v =>
				{
					var cultureName = v.VoiceInfo.Culture.Name;

					return cultureName == "en-US" ||
						   cultureName == "ru-RU" ||
						   cultureName == "cs-CZ";
				});

			if (!voices.Any())
			{
				_logger.LogInformation("No acceptable voice is found.");
				return;
			}

			var settings = _services.GetRequiredService<IApplicationSettingsService>();
			settings.SetCurrentSection("AssistantVoice");

			var voiceName = voices.First().VoiceInfo.Name;
			await settings.SetValueAsync("SelectedVoice", voiceName);

			await Task.Delay(2100);

			_logger.LogInformation("Synthesizer voice selected.");
		}
	}
}
