using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAssistant.Misc
{
	internal class WpfStarter : IHostedService
	{
		private readonly Thread _thread;
		private readonly IHostApplicationLifetime _applicationLifetime;

        public WpfStarter(IServiceProvider provider, IHostApplicationLifetime appLifetime)
        {
			_applicationLifetime = appLifetime;

			_thread = new(() =>
			{
				var app = provider.GetRequiredService<App>();

				app.Exit += App_Exit;

				app.Run();
			});
			_thread.SetApartmentState(ApartmentState.STA);
        }

		private void App_Exit(object sender, System.Windows.ExitEventArgs e)
		{
			_applicationLifetime.StopApplication();
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			_thread.Start();

			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}
	}
}
