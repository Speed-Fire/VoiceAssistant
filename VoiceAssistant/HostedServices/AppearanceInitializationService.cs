using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Misc.DictionarySelection;

namespace VoiceAssistant.HostedServices
{
	internal class AppearanceInitializationService(
		ThemeSelector themeSelector,
		LanguageSelector languageSelector,
		ILogger<AppearanceInitializationService> logger)
		: IHostedService
	{
		private readonly ThemeSelector _themeSelector = themeSelector;
		private readonly LanguageSelector _languageSelector = languageSelector;
		private readonly ILogger _logger = logger;

		public Task StartAsync(CancellationToken cancellationToken)
		{
			_logger.LogInformation("Starting application appearance initialization...");

			var themeCount = InitThemes();

			_logger.LogInformation("{themeCount} themes loaded.", themeCount);

			var languageCount = InitLanguages();

			_logger.LogInformation("{languaageCount} languages loaded.", languageCount);

			_logger.LogInformation("Application appearance initialized successfully.");

			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}

		private int InitThemes()
		{
			var entries = Directory.GetFiles("Themes", "*.xaml");
			foreach (var entry in entries)
			{
				_themeSelector.AddSourcePath(entry);
			}

			return entries.Length;
		}

		private int InitLanguages()
		{
			var entries = Directory.GetFiles("Languages", "*.xaml");
			foreach (var entry in entries)
			{
				_languageSelector.AddSourcePath(entry);
			}

			return entries.Length;
		}
	}
}
