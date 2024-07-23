using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Domain.Models;
using VoiceAssistant.ViewModels.AssistantCommands;

namespace VoiceAssistant.Extensions
{
	internal static class EntityExtensions
	{
		internal static AssistantCommand Map(this AssistantCommandEntity entity, AssistantCommand? existing = null)
		{
			var action = existing is null ? new() { Name = string.Empty, Command = string.Empty, Input = [] } : existing;

			action.Id = entity.Id;
			action.Name = entity.Name;
			action.Command = entity.Command;
			action.Description = string.IsNullOrWhiteSpace(entity.Description) ? null : entity.Description;
			action.NeedsConfirmation = entity.NeedsConfirmation;
			action.IsEnabled = entity.IsEnabled;
			action.AssistantScript = entity.Script;
			action.AssistantScriptId = entity.AssistantScriptId;
			action.Input = entity.Input;

			if (action.AssistantScript is not null)
			{
				var oldAction = action.AssistantScript.Actions.FirstOrDefault(a => a.Id == action.Id);

				if (oldAction is not null)
				{
					action.AssistantScript.Actions.Remove(oldAction);
					action.AssistantScript.Actions.Add(action);
				}
			}

			return action;
		}

		internal static AssistantCommandEntity Map(this AssistantCommand command, AssistantCommandEntity? existing = null)
		{
			var entity = existing is null ? new() : existing;

			entity.Id = command.Id;
			entity.Name = command.Name;
			entity.Command = command.Command;
			entity.Description = string.IsNullOrWhiteSpace(command.Description) ? null : command.Description;
			entity.NeedsConfirmation = command.NeedsConfirmation;
			entity.IsEnabled = command.IsEnabled;
			entity.Script = command.AssistantScript;
			entity.AssistantScriptId = command.AssistantScriptId;
			entity.Input = command.Input;

			return entity;
		}
	}
}
