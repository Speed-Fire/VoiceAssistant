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
using VoiceAssistant.Services.AssistantActionServices;
using VoiceAssistant.Services.Entities;
using VoiceAssistant.UI.Common.Collections.Filter;
using VoiceAssistant.UI.Common.Extensions;
using VoiceAssistant.ViewModels.AssistantActions;
using VoiceAssistant.Views;
using VoiceAssistant.Views.AssistantActions;

namespace VoiceAssistant.ViewModels
{
	public partial class AssistantActionsVM(
		IAssistantActionService assistantActionService,
		IUrgentNotifier urgentNotificator) 
		: ViewModel<AssistantActionsView>
	{
		private readonly IAssistantActionService _assistantActionService = assistantActionService;
		private readonly IUrgentNotifier _urgentNotifier = urgentNotificator;

		private volatile bool _initialized = false;
		public FilteringCollection<AssistantActionEntity> AssistantActions { get; } = [];

		public AssistantActionFilterFactory AssistantActionFilterFactory { get; } = new();

		#region Commands

		#region Start up

		[RelayCommand]
		private Task OnLoaded()
		{
			if (_initialized)
				return Task.CompletedTask;

			return Task.Run(async () =>
			{
				var actions = await _assistantActionService.GetAllAsync();

				Dispatcher.Invoke(() =>
				{
					foreach (var action in actions)
					{
						AssistantActions.Add(action);
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
			var filter = AssistantActionFilterFactory.Create();

			AssistantActions.Filter(filter);
		}

		[RelayCommand]
		private void ClearFilter()
		{
			AssistantActionFilterFactory.Clear();

			AssistantActions.ClearFilter();
		}

		#endregion

		#region DAL commands

		[RelayCommand]
		private async Task EnableAction(AssistantActionEntity action)
		{
			var res = await _assistantActionService.UpdateAsync(action);
			if (!res)
			{
				action.IsEnabled = !action.IsEnabled;

				// error handling
				_urgentNotifier
					.NotifyError("Can't turn on this action.");
			}
		}

		[RelayCommand]
		private void CreateAction()
		{
			var vm = new ChangeAssistantActionVM(_assistantActionService, _urgentNotifier);

			this.Navigation.PushDialog<AssistantActionEntity?>(vm, (response) =>
			{
				if (response.Result != true || response.ReturnValue == null)
					return;

				Dispatcher.Invoke(() =>
				{
					AssistantActions.Add(response.ReturnValue);
				});
			});
		}

		[RelayCommand]
		private void EditAction(AssistantActionEntity action)
		{
			var vm = new ChangeAssistantActionVM(_assistantActionService, _urgentNotifier, action);

			this.Navigation.PushDialog<AssistantActionEntity?>(vm, (response) =>
			{
				if (response.Result != true || response.ReturnValue == null)
					return;

				Dispatcher.Invoke(() =>
				{
					var pos = AssistantActions.IndexOf(act => act.Id == response.ReturnValue.Id);
					AssistantActions[pos] = response.ReturnValue;
				});
			});
		}

		[RelayCommand]
		private async Task DeleteAction(AssistantActionEntity action)
		{
			var res = await _assistantActionService.DeleteAsync(action);

			if (res)
			{
				Dispatcher.Invoke(() =>
				{
					AssistantActions.Remove(action);
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

				var action = new AssistantActionEntity()
				{
					AssistantScript = script,
					Name = $"Action {i}",
					Command = "abra kadabra",
					Description = i % 2 == 0 ? $"descr {i}" : null,
					NeedsConfirmation = i % 2 == 1
				};

				AssistantActions.Add(action);
			}
		}
	}
}
