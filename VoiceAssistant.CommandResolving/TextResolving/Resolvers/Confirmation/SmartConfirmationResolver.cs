using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.CommandResolving.Misc;
using VoiceAssistant.CommandResolving.TextResolving.Resolvers.Commands;
using VoiceAssistant.Common;
using VoiceAssistant.Core.Interfaces;

namespace VoiceAssistant.CommandResolving.TextResolving.Resolvers.Confirmation
{
	internal class SmartConfirmationResolver(Func<IChatGPT> chatGPTFactory)
		: ITextResolver<bool?>
	{
		private const string SYSTEM_MESSAGE = "Hi. I will send you sentences in English, Russian or Czech, and your task is to determine whether the sentence is confirmation or refuse. Your answer should be only one number: 0 for refuse; 1 for confirmation; 2 for neither one of both. Remember: ONLY NUMBER, NOTHING MORE. Remember: Russian \"да\" is CONFIRMATION.";

		private readonly Func<IChatGPT> _chatGPTFactory = chatGPTFactory;

		private bool isInitialized = false;
		public bool IsInitialized => isInitialized;

		private IChatGPT? _chat;

		private DateTime? _lastHistoryClean;

		public async Task<bool> Initialize()
		{
			isInitialized = false;

			try
			{
				_chat = _chatGPTFactory.Invoke();
				if (_chat is null)
					return false;

				await _chat.SendMessage(SYSTEM_MESSAGE);

				isInitialized = true;
			}
			catch { }

			return isInitialized;
		}

		public async Task<OneOf<bool?, Exception>> Resolve(string text)
		{
			if (!isInitialized || _chat is null)
			{
				return new(new NotInitializedException(nameof(SmartCommandResolver)));
			}

			try
			{
				await TryClearHistory();

				var response = await _chat.SendMessage(text);
				if (int.TryParse(response, out var number))
				{
					if (number == 0)
						return new(false);
					else if (number == 1)
						return new(true);
					else
						return new((bool?)null);
				}

				return new(new UnrecognizedResponseException());
			}
			catch (Exception ex)
			{
				return new(ex);
			}
		}

		private async Task TryClearHistory()
		{
			if (_lastHistoryClean is null)
			{
				_lastHistoryClean = DateTime.Now;
				return;
			}

			if (DateTime.Now - _lastHistoryClean < TimeSpan.FromHours(1))
				return;

			_lastHistoryClean = DateTime.Now;
			_chat?.ClearHistory();

			await Initialize();
		}

		public void Dispose()
		{
			
		}
	}
}
