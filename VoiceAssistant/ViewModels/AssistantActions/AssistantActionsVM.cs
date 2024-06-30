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
using VoiceAssistant.Core.Misc;
using VoiceAssistant.Domain.Models;
using VoiceAssistant.Misc.FilteringCollection;
using VoiceAssistant.Notifications.Urgent;
using VoiceAssistant.Services.AssistantActionServices;
using VoiceAssistant.Services.Entities;
using VoiceAssistant.ViewModels.AssistantActions;
using VoiceAssistant.Views;
using VoiceAssistant.Views.AssistantActions;

namespace VoiceAssistant.ViewModels
{
	public partial class AssistantActionsVM(
		[FromKeyedServices(NavConsts.SINGLETON_SERVICE)] INavigationService globalNavigation,
		IAssistantActionService assistantActionService,
		IUrgentNotificator urgentNotificator) 
		: ViewModel<AssistantActionsView>
	{
		private readonly INavigationService _globalNavigation = globalNavigation;
		private readonly IAssistantActionService _assistantActionService = assistantActionService;
		private readonly IUrgentNotificator _urgentNotificator = urgentNotificator;

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

					TestElements();
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
				_urgentNotificator
					.NotifyError("Can't turn on this action.", 0);
			}
		}

		[RelayCommand]
		private void CreateAction()
		{
			var vm = new ChangeAssistantActionVM(_assistantActionService);

			_globalNavigation.PushDialog<AssistantActionEntity?>(vm, (response) =>
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
			var vm = new ChangeAssistantActionVM(_assistantActionService, action);

			_globalNavigation.PushDialog<AssistantActionEntity?>(vm, (response) =>
			{
				if (response.Result != true || response.ReturnValue == null)
					return;

				Dispatcher.Invoke(() =>
				{
					var pos = AssistantActions.IndexOf(response.ReturnValue);
					AssistantActions[pos] = response.ReturnValue;
					//AssistantActions.RemoveAt(pos);
					//AssistantActions.Insert(pos, response.ReturnValue);
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
