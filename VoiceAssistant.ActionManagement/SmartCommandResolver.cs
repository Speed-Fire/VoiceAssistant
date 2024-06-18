using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.ActionManagement.Misc;
using VoiceAssistant.Core.Interfaces;
using VoiceAssistant.Core.Misc;
using VoiceAssistant.Domain.Models;

namespace VoiceAssistant.ActionManagement
{
	public class SmartCommandResolver : ICommandResolver
	{
		private const string SYSTEM_MSG = "Hi. I'll send you a enumerated list of possible actions. Then i'm going to send you some sentences and you must send me back the number of the most similar action. If the sentence is not similar to any actions, then send -1. You should send only number.";

		private readonly IChatGPT _chat;
		private readonly Provider<List<AssistantAction>> _actions;

		private bool isInitialized = false;
		public bool IsInitialized => isInitialized;

		private DateTime? _lastHistoryClean;

		public SmartCommandResolver(IChatGPT chat, Provider<List<AssistantAction>> actions)
		{
			_chat = chat;
			_actions = actions;
		}

		public async Task Initialize()
		{
			isInitialized = false;

			try
			{
				await _chat.SendMessage(SYSTEM_MSG);

				await _chat.SendMessage(GetActionsList());

				isInitialized = true;
			}
			catch { }
		}

		public async Task<OneOf<AssistantAction, Exception>> Resolve(string command)
		{
			if(!isInitialized)
			{
				return new(new NotInitializedException(nameof(SmartCommandResolver)));
			}

			try
			{
				await TryClearHistory();

				var response = await _chat.SendMessage(command);
				if (int.TryParse(response, out var number))
				{
					if (number < 0)
						return new(new UnrecognizedCommandException());
					else
#pragma warning disable CS8602 // Разыменование вероятной пустой ссылки.
						return new(_actions.Value[number]);
#pragma warning restore CS8602 // Разыменование вероятной пустой ссылки.
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
			if(_lastHistoryClean is null)
			{
				_lastHistoryClean = DateTime.Now;
				return;
			}

			if (DateTime.Now - _lastHistoryClean < TimeSpan.FromHours(1))
				return;

			_lastHistoryClean = DateTime.Now;
			_chat.ClearHistory();

			await Initialize();
		}

		private string GetActionsList()
		{
			var sb = new StringBuilder();

			var i = 0;
#pragma warning disable CS8602 // Разыменование вероятной пустой ссылки.
			foreach (var action in _actions.Value)
			{
				sb.Append($"{i++}. ");
				sb.AppendLine(action.Command);
			}
#pragma warning restore CS8602 // Разыменование вероятной пустой ссылки.

			return sb.ToString();
		}

		public void Dispose() { }
	}
}
