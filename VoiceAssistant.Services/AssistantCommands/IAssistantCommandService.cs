using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Domain.Models;

namespace VoiceAssistant.Services.AssistantCommands
{
	public interface IAssistantCommandService
	{
		Task<IEnumerable<AssistantCommand>> GetAllAsync();
		Task<bool> CreateAsync(AssistantCommand command);
		Task<bool> UpdateAsync(AssistantCommand command);
		Task<bool> DeleteAsync(AssistantCommand command);
	}
}
