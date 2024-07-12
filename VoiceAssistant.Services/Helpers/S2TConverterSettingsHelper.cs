using Microsoft.Extensions.Options;
using Plugin.S2T.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Core.SettingsHelpers;
using VoiceAssistant.Services.Options;

namespace VoiceAssistant.Services.Helpers
{
	internal class S2TConverterSettingsHelper : IS2TConverterSettingsHelper
	{
		public IEnumerable<string> AvailableConverters { get; }
		public string? SelectedConverter { get; }

        public S2TConverterSettingsHelper(
            IEnumerable<S2TConverterInfo> converterInfos,
            IOptionsMonitor<SpeechToTextOptions> options)
        {
            AvailableConverters = converterInfos.Select(c => c.Name).ToList();
            SelectedConverter = options.CurrentValue.SelectedConverter;
        }
    }
}
