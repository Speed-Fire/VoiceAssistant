using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Synergy.Core.Collections.Observable;
using Synergy.WPF.Navigation.Misc;
using Synergy.WPF.Navigation.Services;
using Synergy.WPF.Navigation.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Common;
using VoiceAssistant.Core.Interfaces;
using VoiceAssistant.Domain.Models;
using VoiceAssistant.Extensions;
using VoiceAssistant.Notifications.Urgent;
using VoiceAssistant.Services.AssistantCommands;
using VoiceAssistant.Services.Entities;
using VoiceAssistant.UI.Common.Collections.Filter;
using VoiceAssistant.UI.Common.Extensions;
using VoiceAssistant.ViewModels.AssistantCommands;
using VoiceAssistant.Views;
using VoiceAssistant.Views.AssistantCommands;

namespace VoiceAssistant.ViewModels
{
	public partial class AssistantCommandsVM(
		IAssistantCommandService assistantCommandService,
		IUrgentNotifier urgentNotificator) 
		: ViewModel<AssistantCommandsView>
	{
		private readonly IAssistantCommandService _assistantCommandService = assistantCommandService;
		private readonly IUrgentNotifier _urgentNotifier = urgentNotificator;

		private volatile bool _initialized = false;
		public FilteringCollection<AssistantCommandEntity> AssistantCommands { get; } = [];

		public AssistantCommandFilterFactory AssistantCommandFilterFactory { get; } = new();

		#region Commands

		#region Start up

		[RelayCommand]
		private Task OnLoaded()
		{
			if (_initialized)
				return Task.CompletedTask;

			return Task.Run(async () =>
			{
				var actions = await _assistantCommandService.GetAllAsync();

				Dispatcher.Invoke(() =>
				{
					foreach (var action in actions)
					{
						AssistantCommands.Add(action);
					}

					//TestElements();
				});

				_initialized = true;
			});
		}

		#endregion

		#region Filter

		[RelayCommand]
		private void ApplyFilter()
		{
			var filter = AssistantCommandFilterFactory.Create();

			AssistantCommands.Filter(filter);
		}

		[RelayCommand]
		private void ClearFilter()
		{
			AssistantCommandFilterFactory.Clear();

			AssistantCommands.ClearFilter();
		}

		#endregion

		#region DAL commands

		[RelayCommand]
		private async Task EnableCommand(AssistantCommandEntity action)
		{
			var res = await _assistantCommandService.UpdateAsync(action);
			if (!res)
			{
				action.IsEnabled = !action.IsEnabled;

				// error handling
				_urgentNotifier
					.NotifyError("Can't turn on this action.");
			}
		}

		[RelayCommand]
		private void CreateCommand()
		{
			var vm = new ChangeAssistantCommandVM(_assistantCommandService, _urgentNotifier);

			this.Navigation.PushDialog<AssistantCommandEntity?>(vm, (response) =>
			{
				if (response.Result != true || response.ReturnValue == null)
					return;

				Dispatcher.Invoke(() =>
				{
					AssistantCommands.Add(response.ReturnValue);
				});
			});
		}

		[RelayCommand]
		private void EditCommand(AssistantCommandEntity action)
		{
			var vm = new ChangeAssistantCommandVM(_assistantCommandService, _urgentNotifier, action);

			this.Navigation.PushDialog<AssistantCommandEntity?>(vm, (response) =>
			{
				if (response.Result != true || response.ReturnValue == null)
					return;

				Dispatcher.Invoke(() =>
				{
					var pos = AssistantCommands.IndexOf(act => act.Id == response.ReturnValue.Id);
					AssistantCommands[pos] = response.ReturnValue;
				});
			});
		}

		[RelayCommand]
		private async Task DeleteCommand(AssistantCommandEntity action)
		{
			var res = await _assistantCommandService.DeleteAsync(action);

			if (res)
			{
				Dispatcher.Invoke(() =>
				{
					AssistantCommands.Remove(action);
				});
			}
			else
			{
				_urgentNotifier
					.NotifyError("Can't delete this action.");
			}
		}

		#endregion

		#endregion

		private void TestElements()
		{
			for (int i = 0; i < 30; i++)
			{
				var script = new AssistantScript() { Id = i, Name = $"Script {i}" };

				var action = new AssistantCommandEntity()
				{
					AssistantScript = script,
					Name = $"Action {i}",
					Command = "abra kadabra",
					Description = i % 2 == 0 ? $"descr {i}" : null,
					NeedsConfirmation = i % 2 == 1
				};

				AssistantCommands.Add(action);
			}
		}
	}
}
