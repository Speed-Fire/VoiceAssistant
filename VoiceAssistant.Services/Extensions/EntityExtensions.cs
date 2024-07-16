using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Domain.Models;
using VoiceAssistant.Services.Entities;

namespace VoiceAssistant.Services.Extensions
{
	internal static class EntityExtensions
	{
		internal static AssistantCommand Map(this AssistantActionEntity entity, AssistantCommand? existing = null)
		{
			var action = existing is null ? new() { Name = string.Empty, Command = string.Empty} : existing;

			action.Id = entity.Id;
			action.Name = entity.Name;
			action.Command = entity.Command;
			action.Description = string.IsNullOrWhiteSpace(entity.Description) ? null : entity.Description;
			action.NeedsConfirmation = entity.NeedsConfirmation;
			action.IsEnabled = entity.IsEnabled;
			action.AssistantScript = entity.AssistantScript;
			action.AssistantScriptId = entity.AssistantScriptId;

			return action;
		}

		internal static AssistantActionEntity Map(this AssistantCommand entity, AssistantActionEntity? existing = null)
		{
			var action = existing is null ? new() : existing;

			action.Id = entity.Id;
			action.Name = entity.Name;
			action.Command = entity.Command;
			action.Description = string.IsNullOrWhiteSpace(entity.Description) ? null : entity.Description;
			action.NeedsConfirmation = entity.NeedsConfirmation;
			action.IsEnabled = entity.IsEnabled;
			action.AssistantScript = entity.AssistantScript;
			action.AssistantScriptId = entity.AssistantScriptId;

			return action;
		}
	}
}
