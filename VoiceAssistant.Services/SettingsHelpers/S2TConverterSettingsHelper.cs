using Microsoft.Extensions.Options;
using Plugin.S2T.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Core.SettingsHelpers;
using VoiceAssistant.Services.Options;

namespace VoiceAssistant.Services.SettingsHelpers
{
	internal class S2TConverterSettingsHelper : IS2TConverterSettingsHelper
	{
		public IEnumerable<string> AvailableConverters { get; }
		public string? SelectedConverter { get; }

        public S2TConverterSettingsHelper(
            IEnumerable<S2TConverterInfo> converterInfos,
            IOptions<SpeechToTextOptions> options)
        {
            AvailableConverters = converterInfos.Select(c => c.Name).ToList();
            SelectedConverter = options.Value.SelectedConverter;
        }
    }
}
