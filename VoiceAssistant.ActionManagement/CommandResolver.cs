using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.ActionManagement.Misc;
using VoiceAssistant.Core.Misc;
using VoiceAssistant.Domain.Models;

namespace VoiceAssistant.ActionManagement
{
	public class CommandResolver(SmartCommandResolver smartResolver, 
		DummyCommandResolver dummyResolver) : ICommandResolver
	{
		private readonly SmartCommandResolver _smartResolver = smartResolver;
		private readonly DummyCommandResolver _dummyResolver = dummyResolver;

		private ICommandResolver _activeResolver = smartResolver;
		private DateTime? _lastSmartResolverCriticalError;

		private readonly Mutex _lock = new(false, ActionConsts.RESOLVER_MUTEX);

		public bool IsInitialized => true;

		public async Task Initialize()
		{
			var smart = _smartResolver.Initialize();
			var dummy = _dummyResolver.Initialize();

			await smart;
			await dummy;

			if(!_smartResolver.IsInitialized)
				SetDummyResolver();
		}

		public async Task<OneOf<AssistantAction, Exception>> Resolve(string command)
		{
			_lock.WaitOne();

			TrySetSmartResolver();

			var result = await _activeResolver.Resolve(command);

			if (result.IsFirst || result.Second is VoicableException)
			{
				_lock.ReleaseMutex();

				return result; 
			}

			SetDummyResolver();

			result = await _activeResolver.Resolve(command);

			_lock.ReleaseMutex();

			return result;
		}

		private void TrySetSmartResolver()
		{
			if (!_smartResolver.IsInitialized || _lastSmartResolverCriticalError is null)
				return;

			if (DateTime.Now - _lastSmartResolverCriticalError < TimeSpan.FromMinutes(5))
				return;

			_lastSmartResolverCriticalError = null;
			_activeResolver = _smartResolver;
		}

		private void SetDummyResolver()
		{
			_lastSmartResolverCriticalError = DateTime.Now;
			_activeResolver = _dummyResolver;
		}

		public void Dispose()
		{
			_smartResolver.Dispose();
			_dummyResolver.Dispose();
			_lock.Dispose();
		}
	}
}
