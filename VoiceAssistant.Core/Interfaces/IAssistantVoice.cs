using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Common;

namespace VoiceAssistant.Core.Interfaces
{
	public interface IAssistantVoice : IDisposable
	{
		Task Initialize();
		Task Speak(string key);
		Task Speak(VoicableException exception);
	}
}
