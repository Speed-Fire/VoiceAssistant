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
		Task<Exception?> AddAsync(AssistantCommandEntity entity);
		Task<Exception?> UpdateAsync(AssistantCommandEntity entity);
		Task<Exception?> DeleteAsync(AssistantCommandEntity entity);
	}
}
