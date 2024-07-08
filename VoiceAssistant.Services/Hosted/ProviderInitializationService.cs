using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Services.Misc.Interfaces;

namespace VoiceAssistant.Services.Hosted
{
	public class ProviderInitializationService(
		ILogger<ProviderInitializationService> logger,
		IServiceProvider serviceProvider)
		: IHostedService
	{
		private readonly ILogger _logger = logger;
		private readonly IServiceProvider _serviceProvider = serviceProvider;

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			_logger.LogInformation("Starting providers initialization...");

			var initializers = _serviceProvider.GetServices<IProviderInitializer>();

			var count = 0;
			foreach (var initializer in initializers)
			{
				await initializer.InitializeAsync();

				count++;
			}

			_logger.LogInformation("{count} providers are initialized.", count);
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}
	}
}
