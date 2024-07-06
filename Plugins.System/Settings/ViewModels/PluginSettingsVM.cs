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

namespace PluginsSystem.Settings.ViewModels {

	public partial class PluginSettingsVM 
		: ViewModel<PluginSettingsView>
	{
		private readonly IUrgentNotifier _urgentNotifier;
		private readonly PluginInfoEntity _pluginInfo;
		private readonly SettingsRepository _repository;

		public PluginInfoEntity PluginInfo => _pluginInfo;
		public IReadOnlyList<BindableProperty> BindableProperties { get; }

		private volatile bool _askForNotSaved = false;

        public PluginSettingsVM(
			IUrgentNotifier urgentNotifier,
			PluginInfoEntity pluginInfo)
        {
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
				property.PropertyChanged += (sender, e) => { _askForNotSaved = true; };

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
		private void Cancel()
		{
			if (_askForNotSaved)
			{
				_urgentNotifier.NotifyWarning(GetAppResource<string>("Strings.Errors.Changes.Unsaved"),
					5000);

				_askForNotSaved = false;

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
