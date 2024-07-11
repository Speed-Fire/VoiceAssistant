using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAssistant.SpeechSynthesis.Options
{
	public class AssistantVoiceOptions
	{
		public string SelectedVoice { get; set; } = string.Empty;
		public int Volume { get; set; }
	}
}
