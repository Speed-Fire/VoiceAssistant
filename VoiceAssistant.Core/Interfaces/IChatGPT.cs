using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAssistant.Core.Interfaces
{
	public interface IChatGPT
	{
		Task AddSystemMessage(string message);
		void ClearHistory();
		Task<string?> SendMessage(string message);
		Task<IReadOnlyList<string>> SendMessageForMultipleAnswers(string message);
	}
}
