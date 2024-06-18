using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.ActionManagement.Misc;
using VoiceAssistant.Core.Misc;
using VoiceAssistant.Domain.Models;

namespace VoiceAssistant.ActionManagement
{
	public class DummyCommandResolver : ICommandResolver
	{
		private readonly Provider<List<AssistantAction>> _actions;

		public DummyCommandResolver(Provider<List<AssistantAction>> actions)
		{
			_actions = actions;
		}

		public Task Initialize()
		{
			return Task.CompletedTask;
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
			var action = _actions.Value.FirstOrDefault(x => string.Equals(x.Command, command,
				StringComparison.OrdinalIgnoreCase));

			if (action is null)
				result = new(new UnrecognizedCommandException());
			else
				result = new(action);

			finish:
			return Task.FromResult(result);
		}

		public void Dispose() { }
	}
}
