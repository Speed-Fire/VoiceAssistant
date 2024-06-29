using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Domain.Models;
using VoiceAssistant.Services.Entities;

namespace VoiceAssistant.Services.AssistantActionServices
{
	public interface IAssistantActionService
	{
		Task<IEnumerable<AssistantActionEntity>> GetAllAsync();
		Task<bool> CreateAsync(AssistantActionEntity action);
		Task<bool> UpdateAsync(AssistantActionEntity action);
		Task<bool> DeleteAsync(AssistantActionEntity action);
	}
}
