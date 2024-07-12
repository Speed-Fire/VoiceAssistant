using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plugin.S2T.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Common;
using VoiceAssistant.Core.Interfaces;
using VoiceAssistant.Core.Models;
using VoiceAssistant.DAL.Repositories;
using VoiceAssistant.Services.Misc.Interfaces;
using VoiceAssistant.Services.Options;

namespace VoiceAssistant.Services.Hosted
{
    public class S2TConverterService(
		ILogger<S2TConverterService> logger,
		IServiceProvider services,
		Provider<IS2TConverter> converterProvider,
		IEnumerable<S2TConverterInfo> converterInfos,
		IOptionsMonitor<SpeechToTextOptions> options) 
        : ISequentialInitializer
    {
        private readonly ILogger _logger = logger;
        private readonly IServiceProvider _services = services;
        private readonly Provider<IS2TConverter> _converterProvider = converterProvider;
        private readonly IEnumerable<S2TConverterInfo> _converterInfos = converterInfos;
        private readonly IOptionsMonitor<SpeechToTextOptions> _options = options;

		public async Task Initialize()
		{
            var res = await InitializeInternal();

            _options.OnChange(OptionsChanged);
		}

        private void OptionsChanged(SpeechToTextOptions options)
        {
            var info = _converterInfos.FirstOrDefault(i => i.Name == options.SelectedConverter);
            if(info is null)
            {

                return;
            }

            _converterProvider.Value = info.ConverterFactory.Invoke();
        }

		#region Initialization

		public async Task<bool> InitializeInternal()
        {
            var settings = _services.GetRequiredService<IApplicationSettingsService>();
			settings.SetCurrentSection("SpeechToText");

			_logger.LogInformation("Starting S2TConverter provider initialization...");

            bool result = true;

            if (!string.IsNullOrWhiteSpace(_options.CurrentValue.SelectedConverter))
            {
                if (!await TrySetSelectedConverter(settings))
                {
                    result = await SetFirstSatisfyingConverter(settings);
                }
            }
            else
            {
                _logger.LogInformation("Converter is not selected.");

                result = await SetFirstSatisfyingConverter(settings);
            }

            if (result)
                _logger.LogInformation("S2TConverter provider initialization finished.");
            else
                _logger.LogInformation("None converter can be initialized or there is no any registered converters.");

            return result;
        }

        private async Task<bool> SetFirstSatisfyingConverter(IApplicationSettingsService settings)
        {
            foreach (var converterInfo in _converterInfos)
            {
                _logger.LogInformation("Searching for converter...");

                if (TryInitializeConverter(converterInfo))
                {
                    await settings.SetValueAsync("SelectedConverter", converterInfo.Name);

                    return true;
                }
            }

            return false;
        }

        private async Task<bool> TrySetSelectedConverter(IApplicationSettingsService settings)
        {
            _logger.LogInformation("Searching for selected converter...");

            var converterInfo =
                _converterInfos.FirstOrDefault(info => info.Name == _options.CurrentValue.SelectedConverter);

            if (converterInfo is null)
            {
                _logger.LogInformation("Selected converter is not found.");

                await settings.SetValueAsync("SelectedConverter", string.Empty);
            }
            else if (TryInitializeConverter(converterInfo))
            {
                return true;
            }

            _logger.LogInformation("Selected converter can't be set.");

            await settings.SetValueAsync("SelectedConverter", string.Empty);

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

		#endregion
	}
}
