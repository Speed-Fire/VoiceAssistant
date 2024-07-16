using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.CommandResolving.Switch;
using VoiceAssistant.Common;
using VoiceAssistant.Domain.Models;
using VoiceAssistant.Domain.Underlying;

namespace VoiceAssistant.CommandResolving.TextResolving.Services
{
	internal class TextResolvingService(
		ActiveTextResolverSwitch<bool?> confirmationResolverSwitch, 
		ActiveTextResolverSwitch<UnderlyingCommand> commandResolverSwitch)
		: ITextResolvingService
	{
		private readonly ActiveTextResolverSwitch<bool?> _confirmationResolverSwitch 
			= confirmationResolverSwitch;
		private readonly ActiveTextResolverSwitch<UnderlyingCommand> _commandResolverSwitch 
			= commandResolverSwitch;
		private readonly SemaphoreSlim _semaphore = new(1, 1);

		public async Task<bool> Initialize()
		{
			var confirmationResolversInitialization = _confirmationResolverSwitch.Initialize();
			var commandResolversInitialization = _commandResolverSwitch.Initialize();

			return await confirmationResolversInitialization &&
				   await commandResolversInitialization;
		}

		public Task<OneOf<UnderlyingCommand, Exception>> ResolveCommand(string text)
		{
			return Resolve(_commandResolverSwitch, text);
		}

		public Task<OneOf<bool?, Exception>> ResolveConfirmation(string text)
		{
			return Resolve(_confirmationResolverSwitch, text);
		}

		public async Task<OneOf<T, Exception>> Resolve<T>(
			ActiveTextResolverSwitch<T> resolverSwitch, 
			string text)
		{
			await _semaphore.WaitAsync();

			var resolver = resolverSwitch.GetActiveResolver();

			var result = await resolver.Resolve(text);

			if (result.IsFirst || result.Second is VoicableException)
			{
				_semaphore.Release();

				return result;
			}

			resolverSwitch.SignalCurrentResolverError();
			resolver = resolverSwitch.GetActiveResolver();

			result = await resolver.Resolve(text);

			_semaphore.Release();

			return result;
		}

		public void Dispose()
		{
			_semaphore.Dispose();
		}
	}
}
