using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Domain.Models;
using VoiceAssistant.Services.Entities;

namespace VoiceAssistant.Services.AssistantCommands
{
	public interface IAssistantCommandService
	{
		Task<IEnumerable<AssistantCommandEntity>> GetAllAsync();
		Task<bool> CreateAsync(AssistantCommandEntity action);
		Task<bool> UpdateAsync(AssistantCommandEntity action);
		Task<bool> DeleteAsync(AssistantCommandEntity action);
	}
}
