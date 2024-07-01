using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.ActionManagement.Misc;
using VoiceAssistant.ActionManagement.Options;

namespace VoiceAssistant.ActionManagement.Switch
{
	internal class ActiveCommandResolverSwitch(
		Func<IEnumerable<ICommandResolver>> commandResolversFactory,
		IOptionsMonitor<CommandResolverOptions> options)
		: IActiveCommandResolverSwitch
	{
		private readonly IOptionsMonitor<CommandResolverOptions> _options = options;
		private readonly Func<IEnumerable<ICommandResolver>> _commandResolversFactory
			= commandResolversFactory;

#nullable disable

		private ICommandResolver _smartResolver;
		private ICommandResolver _dummyResolver;

#nullable enable

		private readonly List<ICommandResolver> _availableResolvers = [];
		public IEnumerable<string> AvailableResolvers =>
			_availableResolvers.Select(r => r.Name);

		private DateTime? _lastPreferredResolverCriticalError;

		async Task<bool> IActiveCommandResolverSwitch.Initialize()
		{
			var resolvers = _commandResolversFactory.Invoke();

			if(!resolvers.Any())
				return false;

			var atLestOneInitialized = false;

			foreach(var resolver in resolvers)
			{
				switch(resolver)
				{
					case SmartCommandResolver:
						_smartResolver = resolver;
						break;

					case DummyCommandResolver:
						_dummyResolver = resolver;
						break;

					default:
						break;
				}

				var initResult = await resolver.Initialize();
				if (!initResult)
					continue;

				atLestOneInitialized = true;
				_availableResolvers.Add(resolver);
			}

			if(_dummyResolver is null || !atLestOneInitialized)
				return false;

			return true;
		}

		ICommandResolver IActiveCommandResolverSwitch.GetActiveResolver()
		{
			var auto = _options.CurrentValue.AutomaticResolverSelection;
			var name = _options.CurrentValue.SelectedResolver;

			var resolver = auto ? _smartResolver : _availableResolvers
				.Where(r => r.Name == name)
				.FirstOrDefault();

			if (TrySetPreferredResolver(_smartResolver))
				return _smartResolver;

			return _dummyResolver;
		}

		private bool TrySetPreferredResolver(ICommandResolver resolver)
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

		void IActiveCommandResolverSwitch.SignalCurrentResolverError()
		{
			_lastPreferredResolverCriticalError = DateTime.Now;
		}
	}
}
