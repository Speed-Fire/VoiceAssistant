using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Synergy.WPF.Navigation.Services;
using System.Configuration;
using System.Data;
using System.Windows;
using VoiceAssistant.Misc;
using VoiceAssistant.Misc.Helpers;
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

		public App(
			IServiceProvider serviceProvider)
		{
			InitializeComponent();

			_services = serviceProvider;
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			var window = _services.GetRequiredService<MainWindow>();
			MainWindow = window;

			WindowHelper.Init(window);

			MainWindow.Show();
		}
	}

}
