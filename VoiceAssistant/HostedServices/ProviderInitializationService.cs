using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Services.Misc.Interfaces;

namespace VoiceAssistant.HostedServices
{
	internal class ProviderInitializationService(
		ILogger<ProviderInitializationService> logger,
		IEnumerable<IProviderInitializer> initializers)
		: IHostedService
	{
		private readonly ILogger _logger = logger;
		private readonly IEnumerable<IProviderInitializer> _initializers = initializers;

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			_logger.LogInformation("Starting providers initialization...");

			var count = 0;
			foreach (var initializer in _initializers)
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
