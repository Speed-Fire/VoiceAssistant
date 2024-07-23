using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Domain.Models;

#nullable disable

namespace VoiceAssistant.Domain.Underlying
{
	public class UnderlyingCommand(long id)
	{
		public long Id { get; } = id;
		public string Command { get; set; }
		public IEnumerable<string> Input { get; set; }
		public bool IsEnabledUser { get; set; }
		public bool NeedsConfirmation { get; set; }

		public UnderlyingScript Script { get; set; }

		public bool IsEnabled => IsEnabledUser && Script is not null;

		public static implicit operator UnderlyingCommand(AssistantCommand command)
			=> new(command.Id)
			{
				Command = command.Command,
				Input = command.Input,
				IsEnabledUser = command.IsEnabled,
				NeedsConfirmation = command.NeedsConfirmation,
			};
	}
}
