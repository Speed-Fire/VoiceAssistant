using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.CommandResolving.Misc;
using VoiceAssistant.CommandResolving.Switch;
using VoiceAssistant.Common;
using VoiceAssistant.Domain.Models;

namespace VoiceAssistant.CommandResolving
{
	public class CommandResolver(IActiveCommandResolverSwitch activeCommandResolverSwitch)
		: ICommandResolver
	{
		private readonly IActiveCommandResolverSwitch _resolverSwitch = activeCommandResolverSwitch;
		private readonly Mutex _lock = new(false, ActionConsts.RESOLVER_MUTEX);

		public bool IsInitialized => true;

		public string Name => "Main Command Resolver";

		public Task<bool> Initialize()
		{
			return _resolverSwitch.Initialize();
		}

		public async Task<OneOf<AssistantAction, Exception>> Resolve(string command)
		{
			_lock.WaitOne();

			var resolver = _resolverSwitch.GetActiveResolver();

			var result = await resolver.Resolve(command);

			if (result.IsFirst || result.Second is VoicableException)
			{
				_lock.ReleaseMutex();

				return result; 
			}

			_resolverSwitch.SignalCurrentResolverError();
			resolver = _resolverSwitch.GetActiveResolver();

			result = await resolver.Resolve(command);

			_lock.ReleaseMutex();

			return result;
		}

		public void Dispose()
		{
			_lock.Dispose();
		}
	}
}
