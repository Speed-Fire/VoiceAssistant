using Microsoft.Extensions.DependencyInjection;
using Synergy.WPF.Navigation.Services;
using System.Configuration;
using System.Data;
using System.Windows;
using VoiceAssistant.ViewModels;

namespace VoiceAssistant
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private readonly IServiceProvider _services;

		public App(IServiceProvider serviceProvider)
		{
			_services = serviceProvider;
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			var window = _services.GetRequiredService<MainWindow>();
			MainWindow = window;

			var navService = _services.GetRequiredKeyedService<INavigationService>(
				Synergy.WPF.Navigation.Misc.NavConsts.SINGLETON_SERVICE);
			navService.NavigateTo<MainVM>();

			MainWindow.Show();
		}
	}

}
