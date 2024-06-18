using VoiceAssistant.Core.Misc;
using VoiceAssistant.Domain.Models;

namespace VoiceAssistant.ActionManagement
{
	public interface ICommandResolver : IDisposable
	{
		public bool IsInitialized { get; }
		Task Initialize();
		Task<OneOf<AssistantAction, Exception>> Resolve(string command);
	}
}