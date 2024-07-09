using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.CommandResolving.Misc;
using VoiceAssistant.Core.Interfaces;
using VoiceAssistant.Common;
using VoiceAssistant.Domain.Models;

namespace VoiceAssistant.CommandResolving
{
	internal class SmartCommandResolver : ICommandResolver
	{
		private const string SYSTEM_MSG = "Hi. I'll send you a enumerated list of possible actions. Then i'm going to send you some sentences and you must send me back the number of the most similar action. If the sentence is not similar to any actions, then send -1. You should send only number.";

		public string Name => "Smart command resolver";

		private readonly Func<IChatGPT> _chatGPTFactory;
		private readonly Provider<List<AssistantAction>> _actions;
		private readonly Mutex _lock = new(false, ActionConsts.RESOLVER_MUTEX);

		private bool isInitialized = false;
		public bool IsInitialized => isInitialized;

		private IChatGPT? _chat;

		private DateTime? _lastHistoryClean;
		private IEnumerable<AssistantAction> _enabledActions;

		public SmartCommandResolver(Func<IChatGPT> chatGPTFactpry,
			Provider<List<AssistantAction>> actions)
		{
			_chatGPTFactory = chatGPTFactpry;
			_actions = actions;

			_enabledActions = _actions.Value is null ? [] : _actions.Value.Where(a => a.IsEnabled);

			_actions.PropertyChanged += ActionsProvider_Updated;
		}

		public async Task<bool> Initialize()
		{
			isInitialized = false;

			try
			{
				_chat = _chatGPTFactory.Invoke();
				if (_chat is null)
					return false;

				await _chat.SendMessage(SYSTEM_MSG);

				await _chat.SendMessage(GetActionsList());

				isInitialized = true;
			}
			catch { }

			return isInitialized;
		}

		public async Task<OneOf<AssistantAction, Exception>> Resolve(string command)
		{
			if(!isInitialized || _chat is null)
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
						return new(_enabledActions.ElementAt(number));
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
			_chat?.ClearHistory();

			await Initialize();
		}

		private string GetActionsList()
		{
			var sb = new StringBuilder();

			var i = 0;
			foreach (var action in _enabledActions)
			{
				sb.Append($"{i++}. ");
				sb.AppendLine(action.Command);
			}

			return sb.ToString();
		}

		private async void ActionsProvider_Updated(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			_lock.WaitOne();

			_enabledActions = _actions.Value is null ? [] : _actions.Value.Where(a => a.IsEnabled);

			_lastHistoryClean = DateTime.Now.AddDays(-1);

			await TryClearHistory();

			_lock.ReleaseMutex();
		}

		public void Dispose()
		{
			_actions.PropertyChanged -= ActionsProvider_Updated;
		}
	}
}
