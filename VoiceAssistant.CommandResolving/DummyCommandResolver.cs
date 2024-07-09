using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.CommandResolving.Misc;
using VoiceAssistant.Common;
using VoiceAssistant.Domain.Models;

namespace VoiceAssistant.CommandResolving
{
	internal sealed class DummyCommandResolver : ICommandResolver
	{
		private readonly Provider<List<AssistantAction>> _actions;
		private readonly Mutex _lock = new(false, ActionConsts.RESOLVER_MUTEX);

		public string Name => "Dummy command resolver";

		public bool IsInitialized => true;

		private IEnumerable<AssistantAction> _enabledActions;

		public DummyCommandResolver(Provider<List<AssistantAction>> actions)
		{
			_actions = actions;
			_enabledActions = _actions.Value is null ? [] : _actions.Value.Where(a => a.IsEnabled);

			_actions.PropertyChanged += ActionsProvider_Updated;
		}

		public Task<bool> Initialize()
		{
			return Task.FromResult(true);
		}

		public Task<OneOf<AssistantAction, Exception>> Resolve(string command)
		{
			OneOf<AssistantAction, Exception> result;
			if(_actions.Value is null)
			{
				result = new(new InvalidOperationException("AssistantActions aren't loaded!"));
				goto finish;
			}

			// TODO: использовать нейросеть для удаления лишних звуков (ну, ээээ, эм, ммм и т.д.)
			var action = _enabledActions.FirstOrDefault(x => string.Equals(x.Command, command,
				StringComparison.OrdinalIgnoreCase));

			if (action is null)
				result = new(new UnrecognizedCommandException());
			else
				result = new(action);

			finish:
			return Task.FromResult(result);
		}

		private void ActionsProvider_Updated(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			_lock.WaitOne();

			_enabledActions = _actions.Value is null ? [] : _actions.Value.Where(a => a.IsEnabled);

			_lock.ReleaseMutex();
		}

		public void Dispose()
		{
			_actions.PropertyChanged -= ActionsProvider_Updated;
		}
	}
}
