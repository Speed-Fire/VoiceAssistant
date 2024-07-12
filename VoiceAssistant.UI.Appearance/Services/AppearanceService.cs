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
using System.Windows;

namespace VoiceAssistant.UI.Appearance.Services
{
	public class AppearanceService(
		IOptionsMonitor<AppearanceOptions> options,
		ThemeSelector themeSelector,
		LanguageSelector languageSelector,
		IApplicationSettingsService settingsService,
		IUrgentNotifier urgentNotifier,
		ILogger<AppearanceService> logger)
	{
		private readonly IOptionsMonitor<AppearanceOptions> _options = options;
		private readonly ThemeSelector _themeSelector = themeSelector;
		private readonly LanguageSelector _languageSelector = languageSelector;
		private readonly IApplicationSettingsService _settingsService = settingsService;
		private readonly IUrgentNotifier _urgentNotifier = urgentNotifier;
		private readonly ILogger _logger = logger;

		public void Initialize()
		{
			_logger.LogInformation("Starting application appearance initialization...");

			_settingsService.SetCurrentSection("Appearance");

			InitializeInternal();

			SetCurrentValues();

			_logger.LogInformation("Application appearance initialized successfully.");
		}

		private void SetCurrentValues()
		{
			var theme = _options.CurrentValue.Theme;
			var language = _options.CurrentValue.Language;

			if (!_themeSelector.ContainsKey(theme))
			{
				theme = "DarkTheme";
				_settingsService.SetValueAsync("Theme", theme).Wait();
			}

			if (!_languageSelector.ContainsKey(language))
			{
				language = "en-us";
				_settingsService.SetValueAsync("Language", language).Wait();
			}

			_themeSelector.Select(theme);
			_languageSelector.Select(language);

			_logger.LogInformation("Theme \"{theme} is set.\"", theme);
			_logger.LogInformation("Language \"{language}\" is set.", language);

			Task.Delay(2100).Wait();

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

		private void InitializeInternal()
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
