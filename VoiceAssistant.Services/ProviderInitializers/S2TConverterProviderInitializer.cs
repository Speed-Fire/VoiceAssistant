using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Plugin.S2T.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Common;
using VoiceAssistant.Core.Models;
using VoiceAssistant.DAL.Repositories;
using VoiceAssistant.Services.Misc.Interfaces;

namespace VoiceAssistant.Services.ProviderInitializers
{
	internal class S2TConverterProviderInitializer : IProviderInitializer
	{
		private readonly ILogger _logger;
		private readonly ApplicationSettingsService _settings;
		private readonly Provider<IS2TConverter> _converterProvider;
		private readonly IEnumerable<S2TConverterInfo> _converterInfos;
		private readonly string _selectedConverterName;

		public S2TConverterProviderInitializer(
			ILogger<S2TConverterProviderInitializer> logger,
			ApplicationSettingsService settings,
			Provider<IS2TConverter> converterProvider,
			IEnumerable<S2TConverterInfo> converterInfos,
			IConfiguration configuration)
		{
			_logger = logger;
			_settings = settings;
			_converterProvider = converterProvider;
			_converterInfos = converterInfos;

			_selectedConverterName =
				configuration
				.GetSection("Application:SpeechToText:SelectedConverter")?
				.Value ?? string.Empty;

			_settings.SetCurrentSection("SpeechToText");
		}

		public async Task<bool> InitializeAsync()
		{
			_logger.LogInformation("Starting S2TConverter provider initialization...");

			bool result = true;

			if (!string.IsNullOrWhiteSpace(_selectedConverterName))
			{
				if (!await TrySetSelectedConverter())
				{
					result = await SetFirstSatisfyingConverter();
				}
			}
			else
			{
				_logger.LogInformation("Converter is not selected.");

				result = await SetFirstSatisfyingConverter();
			}

			if (result)
				_logger.LogInformation("S2TConverter provider initialization finished.");
			else
				_logger.LogInformation("None converter can be initialized or there is no any registered converters.");

			return result;
		}

		private async Task<bool> SetFirstSatisfyingConverter()
		{
			foreach (var converterInfo in _converterInfos)
			{
				_logger.LogInformation("Searching for converter...");

				if (TryInitializeConverter(converterInfo))
				{
					await _settings.SetValueAsync("SelectedConverter", converterInfo.Name);

					return true;
				}
			}

			return false;
		}

		private async Task<bool> TrySetSelectedConverter()
		{
			_logger.LogInformation("Searching for selected converter...");

			var converterInfo =
				_converterInfos.FirstOrDefault(info => info.Name == _selectedConverterName);

			if (converterInfo is null)
			{
				_logger.LogInformation("Selected converter is not found.");

				await _settings.SetValueAsync("SelectedConverter", string.Empty);
			}
			else if(TryInitializeConverter(converterInfo))
			{
				return true;
			}

			_logger.LogInformation("Selected converter can't be set.");

			await _settings.SetValueAsync("SelectedConverter", string.Empty);

			return false;
		}

		private bool TryInitializeConverter(S2TConverterInfo converterInfo)
		{
			try
			{
				_logger.LogInformation("Converter found. Initializing...");

				_converterProvider.Value =
					converterInfo.ConverterFactory.Invoke();

				_logger.LogInformation("Converter initialized.");

				return true;
			}
			catch (Exception ex)
			{
				_logger
					.LogError(ex, "Failed to initialize {converterName} converter.", converterInfo.Name);

				return false;
			}
		}
	}
}
