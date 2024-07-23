using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.Base;
using PluginsSystem.Entities;
using PluginsSystem.Extensions;
using PluginsSystem.Settings.Views;
using Synergy.WPF.Navigation.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using VoiceAssistant.Common;
using VoiceAssistant.Core.Interfaces;
using VoiceAssistant.UI.Common.Extensions;
using VoiceAssistant.UI.Common.Messages;

namespace PluginsSystem.Settings.ViewModels {

	public partial class PluginSettingsVM 
		: ViewModel<PluginSettingsView>
	{
		private readonly IMessageService _messageService;
		private readonly IUrgentNotifier _urgentNotifier;
		private readonly PluginInfoEntity _pluginInfo;
		private readonly SettingsRepository _repository;

		public PluginInfoEntity PluginInfo => _pluginInfo;
		public IReadOnlyList<BindableProperty> BindableProperties { get; }

		private bool Changed { get; set; } = false;

        public PluginSettingsVM(
			IMessageService messageService,
			IUrgentNotifier urgentNotifier,
			PluginInfoEntity pluginInfo)
        {
			_messageService = messageService;
			_urgentNotifier = urgentNotifier;

            _pluginInfo = pluginInfo;
			_repository = new(GetFilePath(pluginInfo));
			BindableProperties = Initialize(pluginInfo.Parameters);
        }

        private List<BindableProperty> Initialize(
			IReadOnlyList<ParameterInfoEntity> parameterInfos)
		{
			var result = new List<BindableProperty>();

			foreach (var parameterInfo in parameterInfos)
			{
				var value = _repository.GetValue(parameterInfo) ?? string.Empty;

				var property = new BindableProperty(parameterInfo, value);
				property.ErrorsChanged += (sender, e) => { SaveCommand.NotifyCanExecuteChanged(); };

				PropertyChangedEventHandler handler = null!;
				handler = (sender, e) =>
				{
					Changed = true;
					property.PropertyChanged -= handler;
				};
				property.PropertyChanged += handler;

				result.Add(property);
			}
			
			return result;
		}

		[RelayCommand(CanExecute = nameof(CanSaveCommandExecute))]
		private Task Save()
		{
			return Task.Run(async () =>
			{
				foreach (var property in BindableProperties)
				{
					if (!property.Changed)
						continue;

					try
					{
						_repository.SetValue(property.ParameterInfo, property.Value!);
					}
					catch(System.Reflection.TargetInvocationException ex)
						when (ex.InnerException is OverflowException)
					{
						var errorPattern = GetAppResource<string>("Strings.Plugins.Settings.Overflow");

						_urgentNotifier
						.NotifyError(string.Format(errorPattern, property.ParameterInfo.Title),
						5000);

						return;
					}
					catch
					{
						_urgentNotifier
						.NotifyError(GetAppResource<string>("Strings.Errors.Internal"));

						return;
					}
				}

				await _repository.SaveAsync();

				Dispatcher.Invoke(() =>
				{
					this.Navigation.ReleaseDialog();
				});
			});
		}

		private bool CanSaveCommandExecute()
		{
			return !BindableProperties
				.Select(prop => prop.HasErrors)
				.Aggregate((prop1, prop2) => prop1 || prop2);
		}

		[RelayCommand]
		private void SetDefaults()
		{
			foreach (var property in BindableProperties)
			{
				_repository.SetDefaultValue(property.ParameterInfo);

				var value = property.ParameterInfo.DefaultValue;

				property.Value = value.ToString();
			}
		}

		[RelayCommand]
		private async Task Cancel()
		{
			if (Changed)
			{
				var message = GetAppResource<string>("Strings.Warnings.Unsaved");

				var result = await _messageService
					.ShowQuestionAsync(message, MessageBoxButton.YesNo);

				if (result != MessageBoxResult.Yes)
					return;
			}
			
			this.Navigation.ReleaseDialog();
		}

		private static string GetFilePath(PluginInfoEntity pluginInfo)
		{
			return Path.Combine("Config", $"{pluginInfo.AssemblyName}.json");
		}

		public override void Dispose()
		{
			_repository.Dispose();
		}
	}
}
