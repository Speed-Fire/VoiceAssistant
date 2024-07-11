using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.CommandResolving.Options;
using VoiceAssistant.CommandResolving.TextResolving.Resolvers;

namespace VoiceAssistant.CommandResolving.Switch
{
	internal class ActiveTextResolverSwitch<TResult>
	{
		private const string DUMMY = "dummy";
		private const string SMART = "smart";

		private readonly Dictionary<string, ITextResolver<TResult>> _resolvers;
		private readonly string _preferredResolverKey;

		private DateTime? _lastPreferredResolverCriticalError;

		public ActiveTextResolverSwitch(
			ITextResolver<TResult> dummyResolver,
			ITextResolver<TResult> smartResolver,
			string preferredResolverKey)
		{
			_resolvers = new()
			{
				[SMART] = smartResolver,
				[DUMMY] = dummyResolver,
			};

			preferredResolverKey = preferredResolverKey.ToLower();

			_preferredResolverKey =
				string.IsNullOrWhiteSpace(preferredResolverKey)
					|| (preferredResolverKey == SMART)
					|| (preferredResolverKey != DUMMY) ?
				SMART : DUMMY;
		}

		internal async Task<bool> Initialize()
		{
			var result = false;

			foreach(var resolver in _resolvers.Values)
			{
				result |= await resolver.Initialize();
			}

			return result;
		}

		internal ITextResolver<TResult> GetActiveResolver()
		{
			var preferredResolver = _resolvers[_preferredResolverKey];

			if (TrySetPreferredResolver(preferredResolver))
				return preferredResolver;

			return _resolvers[DUMMY];
		}

		private bool TrySetPreferredResolver(ITextResolver<TResult> resolver)
		{
			if (resolver is null ||
				!resolver.IsInitialized ||
				_lastPreferredResolverCriticalError is null)
				return false;

			if (DateTime.Now - _lastPreferredResolverCriticalError < TimeSpan.FromMinutes(5))
				return false;

			_lastPreferredResolverCriticalError = null;

			return true;
		}

		internal void SignalCurrentResolverError()
		{
			_lastPreferredResolverCriticalError = DateTime.Now;
		}
	}
}
