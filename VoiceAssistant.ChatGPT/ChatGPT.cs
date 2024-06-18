using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Core.Interfaces;

namespace VoiceAssistant.ChatGPT
{
	public class ChatGPT : IChatGPT
	{
		private readonly IChatCompletionService _chatService;
		private readonly ChatHistory _history;

		public ChatGPT(IChatCompletionService chatService)
		{
			_chatService = chatService;

			_history = [];
		}

		public async Task AddSystemMessage(string message)
		{
			_history.AddSystemMessage(message);

			await _chatService.GetChatMessageContentAsync(_history);
		}

		public async Task<string?> SendMessage(string message)
		{
			_history.AddUserMessage(message);

			var response = await _chatService.GetChatMessageContentAsync(_history);

			_history.Add(response);

			return response.InnerContent as string;
		}

		public async Task<IReadOnlyList<string>> SendMessageForMultipleAnswers(string message)
		{
			_history.AddUserMessage(message);

			var response = await _chatService.GetChatMessageContentsAsync(_history);
			var result = new List<string>();

			foreach (var answer in response)
			{
				_history.Add(answer);

				if (answer.InnerContent is not string tmp)
					continue;

				result.Add(tmp);
			}

			return result;
		}

		public void ClearHistory()
		{
			_history.Clear();
		}
	}
}
