using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Common;
using VoiceAssistant.Core.Interfaces;
using VoiceAssistant.Scripts.Models;

namespace VoiceAssistant.Scripts.Definitions.Models
{
	internal class SpeakScriptBlock()
		: ScriptBlock(
			"Speak",
			[Tuple.Create("Text", typeof(string))],
			true,
			false,
			[typeof(IAssistantVoice).Assembly,
			 typeof(VoicableException).Assembly],
			[typeof(IAssistantVoice).Namespace!],
			["IAssistantVoice _assistantVoice;"],
			"await _assistantVoice.Speak(Text);"
			)
	{
	}
}
