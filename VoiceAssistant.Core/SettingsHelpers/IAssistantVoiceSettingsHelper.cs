using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAssistant.Core.SettingsHelpers
{
	public interface IAssistantVoiceSettingsHelper
	{
		IEnumerable<string> AvailableVoices { get; }
		string? SelectedVoice { get; }
		int Volume { get; }
	}
}
