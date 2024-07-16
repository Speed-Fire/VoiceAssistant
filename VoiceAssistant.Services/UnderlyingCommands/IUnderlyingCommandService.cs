using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Services.Entities;

namespace VoiceAssistant.Services.UnderlyingCommands
{
	internal interface IUnderlyingCommandService
	{
		Task<Exception?> AddAsync(AssistantActionEntity entity);
		Task<Exception?> UpdateAsync(AssistantActionEntity entity);
		Task<Exception?> DeleteAsync(AssistantActionEntity entity);
	}
}
