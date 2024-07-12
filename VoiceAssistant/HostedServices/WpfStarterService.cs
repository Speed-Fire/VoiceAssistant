using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.UI.Appearance.Services;

namespace VoiceAssistant.HostedServices
{
    internal class WpfStarterService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly Thread _thread;
        private readonly IHostApplicationLifetime _applicationLifetime;
        
        public WpfStarterService(
            IServiceProvider provider,
            IHostApplicationLifetime appLifetime,
            AppearanceService appearanceService,
            ILogger<WpfStarterService> logger)
        {
            _applicationLifetime = appLifetime;
            _logger = logger;

            _thread = new(() =>
            {
                var app = provider.GetRequiredService<App>();

                appearanceService.Initialize();

                app.Exit += App_Exit;

                _logger.LogInformation("Starting WPF.");

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
