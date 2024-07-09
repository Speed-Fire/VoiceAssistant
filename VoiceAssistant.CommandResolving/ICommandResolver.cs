using VoiceAssistant.Common;
using VoiceAssistant.Domain.Models;

namespace VoiceAssistant.CommandResolving
{
	public interface ICommandResolver : IDisposable
	{
		public string Name { get; }
		public bool IsInitialized { get; }
		Task<bool> Initialize();
		Task<OneOf<AssistantAction, Exception>> Resolve(string command);
	}
}