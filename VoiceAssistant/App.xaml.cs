using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Synergy.WPF.Navigation.Services;
using System.Configuration;
using System.Data;
using System.Windows;
using VoiceAssistant.Misc.DictionarySelection;
using VoiceAssistant.Misc.Helpers;
using VoiceAssistant.Misc.Options;
using VoiceAssistant.Notifications;
using VoiceAssistant.ViewModels;

namespace VoiceAssistant
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
	{
		private readonly IServiceProvider _services;
		private readonly ThemeSelector _themeSelector;
		private readonly LanguageSelector _languageSelector;

		public App(IServiceProvider serviceProvider,
			IOptions<InitializationConfig> options)
		{
			InitializeComponent();

			_services = serviceProvider;
			_themeSelector = _services.GetRequiredService<ThemeSelector>();
			_languageSelector = _services.GetRequiredService<LanguageSelector>();

			_themeSelector.Select(options.Value.Theme);
			_languageSelector.Select(options.Value.Language);
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			var window = _services.GetRequiredService<MainWindow>();
			MainWindow = window;

			WindowHelper.Init(window);

			var navService = _services.GetRequiredKeyedService<INavigationService>(
				Synergy.WPF.Navigation.Misc.NavConsts.SINGLETON_SERVICE);
			navService.NavigateTo<MainVM>();

			var urgentNotificationHandler = _services.GetRequiredService<UrgentNotificationHandler>();
			urgentNotificationHandler.StartAsync();

			MainWindow.Show();
		}
	}

}
