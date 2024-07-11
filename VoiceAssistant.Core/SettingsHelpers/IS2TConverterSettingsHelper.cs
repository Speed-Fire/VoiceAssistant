using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAssistant.Core.SettingsHelpers
{
	public interface IS2TConverterSettingsHelper
	{
		IEnumerable<string> AvailableConverters { get; }
		string? SelectedConverter { get; }
	}
}
