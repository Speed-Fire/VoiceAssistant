using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Core.Interfaces;
using VoiceAssistant.UI.Appearance.DictionarySelection;
using VoiceAssistant.UI.Appearance.Options;
using VoiceAssistant.Services;

namespace VoiceAssistant.UI.Appearance.HostedServices
{
	public class AppearanceService(
		IOptionsMonitor<AppearanceOptions> options,
		ThemeSelector themeSelector,
		LanguageSelector languageSelector,
		IApplicationSettingsService settingsService,
		IUrgentNotifier urgentNotifier,
		ILogger<AppearanceService> logger)
		: IHostedService
	{
		private readonly IOptionsMonitor<AppearanceOptions> _options = options;
		private readonly ThemeSelector _themeSelector = themeSelector;
		private readonly LanguageSelector _languageSelector = languageSelector;
		private readonly IApplicationSettingsService _settingsService = settingsService;
		private readonly IUrgentNotifier _urgentNotifier = urgentNotifier;
		private readonly ILogger _logger = logger;

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			_logger.LogInformation("Starting application appearance initialization...");

			_settingsService.SetCurrentSection("Appearance");

			Initialize();

			await SetCurrentValues();

			_logger.LogInformation("Application appearance initialized successfully.");
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}

		private async Task SetCurrentValues()
		{
			var theme = _options.CurrentValue.Theme;
			var language = _options.CurrentValue.Language;

			if (!_themeSelector.ContainsKey(theme))
			{
				theme = "DarkTheme";
				await _settingsService.SetValueAsync("Theme", theme);
			}

			if (!_languageSelector.ContainsKey(language))
			{
				language = "en-us";
				await _settingsService.SetValueAsync("Language", language);
			}

			_themeSelector.Select(theme);
			_languageSelector.Select(language);

			_logger.LogInformation("Theme \"{theme} is set.\"", theme);
			_logger.LogInformation("Language \"{language}\" is set.", language);

			await Task.Delay(2100);

			_options.OnChange(AppearanceChanged);
		}

		private void AppearanceChanged(AppearanceOptions config)
		{
			if (_themeSelector.ContainsKey(config.Theme))
			{
				_themeSelector.Select(config.Theme);
			}
			else
			{
				_urgentNotifier.NotifyError("Can't find selected theme!");
			}

			if (_languageSelector.ContainsKey(config.Language))
			{
				_languageSelector.Select(config.Language);
			}
			else
			{
				_urgentNotifier.NotifyError("Can't find selected language!");
			}

			_logger.LogInformation("Application appearance changed.");
		}

		#region Initialization

		private void Initialize()
		{
			var themeCount = InitThemes();

			_logger.LogInformation("{themeCount} themes loaded.", themeCount);

			var languageCount = InitLanguages();

			_logger.LogInformation("{languaageCount} languages loaded.", languageCount);
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

		#endregion
	}
}
