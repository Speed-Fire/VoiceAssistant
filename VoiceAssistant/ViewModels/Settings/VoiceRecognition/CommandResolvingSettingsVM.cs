using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Options;
using Synergy.WPF.Navigation.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.CommandResolving;
using VoiceAssistant.CommandResolving.Options;
using VoiceAssistant.CommandResolving.Switch;
using VoiceAssistant.Core.Interfaces;
using VoiceAssistant.Services;

namespace VoiceAssistant.ViewModels.Settings.VoiceRecognition
{
	internal partial class CommandResolvingSettingsVM : ViewModel
	{
		private readonly ApplicationSettingsService _settings;
		private readonly IUrgentNotifier _notifier;

		public IEnumerable<string> AvailableResolvers { get; }

		[ObservableProperty]
		private string? _selectedResolver;

		[ObservableProperty]
		private bool _automaticResolverSelection;

		public CommandResolvingSettingsVM(
			ApplicationSettingsService settings,
			IUrgentNotifier notifier,
			IActiveCommandResolverSwitch resolverSwitch,
			IOptions<CommandResolverOptions> options)
		{
			_settings = settings;
			_notifier = notifier;

			AvailableResolvers = resolverSwitch.AvailableResolvers;

			_settings.SetCurrentSection("CommandResolverOptions");

			_selectedResolver = AvailableResolvers
				.FirstOrDefault(r => r == options.Value.SelectedResolver);

			_automaticResolverSelection = options.Value.AutomaticResolverSelection;
		}

		async partial void OnAutomaticResolverSelectionChanged(bool value)
		{
			var result = await _settings.SetValueAsync("AutomaticResolverSelection", value.ToString());
			if(result is not null)
			{
				_notifier
					.NotifyError("Cannot change automatic resolver selection!", exception: result);
			}
		}

		async partial void OnSelectedResolverChanged(string? value)
		{
			var result = await _settings.SetValueAsync("SelectedResolver", value ?? string.Empty);
			if (result is not null)
			{
				_notifier
					.NotifyError("Cannot change selected resolver!", exception: result);
			}
		}
	}
}
