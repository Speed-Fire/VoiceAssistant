using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Domain.Models;

namespace VoiceAssistant.Services.UnderlyingCommands
{
	internal interface IUnderlyingCommandService
	{
		Task<Exception?> AddAsync(AssistantCommand command);
		Task<Exception?> UpdateAsync(AssistantCommand command);
		Task<Exception?> DeleteAsync(AssistantCommand command);
	}
}
