using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PluginsSystem.Configuration;
using PluginsSystem.Entities;
using PluginsSystem.Models;
using PluginsSystem.Examining;
using PluginsSystem.Settings.Initialization;
using System.Collections.Generic;
using System.Threading.Tasks;
using VoiceAssistant.Common;
using Plugin.Base;
using Microsoft.Extensions.Logging;
using System;

namespace PluginsSystem
{
	public class PluginLoader(ILoggerFactory loggerFactory)
	{
		private readonly ILoggerFactory _loggerFactory = loggerFactory;
		private readonly ILogger _logger = loggerFactory.CreateLogger<PluginLoader>();

		public async Task LoadAsync(
			IServiceCollection services,
			IConfiguration configuration,
			string pluginFoldePath,
			string configFolderPath)
		{
			_logger.LogInformation("Starting plugins loading...");

			var examiner = new PluginExaminer(
				pluginFoldePath, 
				_loggerFactory.CreateLogger<PluginExaminer>());
			var pluginDatas = examiner.Examine();

			var plugins = await LoadPlugins(
				services, 
				configuration,
				configFolderPath,
				pluginDatas);

			var provider = new Provider<IEnumerable<PluginInfoEntity>>()
			{
				Value = plugins
			};

			services.AddSingleton(provider);

			_logger.LogInformation("Plugins loading is finished.");
		}

		private async Task<List<PluginInfoEntity>> LoadPlugins(
			IServiceCollection services,
			IConfiguration configuration,
			string configFolderPath,
			IEnumerable<PluginData> datas)
		{
			var result = new List<PluginInfoEntity>();

			var configGenerator = new ConfigGenerator(configFolderPath);
			var settingsReader = new PluginSettingsReader();

			var builder = (IConfigurationBuilder)configuration;
			foreach (var pluginData in datas)
			{
				try
				{
					_logger
						.LogInformation(" [{pluginName}] Starting to load plugin...",
						pluginData.PluginInfo.Name);

					// load plugin settings schema.
					_logger
						.LogInformation(" [{pluginName}] Reading config schema...",
						pluginData.PluginInfo.Name);

					var settings = await settingsReader.ReadParameters(pluginData.PluginInfo);

					// create plugin info entity.
					var plugin = new PluginInfoEntity(pluginData.PluginInfo, settings);
					pluginData.PluginInfo.Dispose();

					_logger
						.LogInformation(" [{pluginName}] Trying to create config file...",
						pluginData.PluginInfo.Name);

					// try to create plugin config.
					var configNeeded = configGenerator.TryCreateConfig(plugin, out var configPath);

					if (configNeeded)
						_logger.LogInformation(" [{pluginName}] Config file created.",
						pluginData.PluginInfo.Name);
					else
						_logger.LogInformation(" [{pluginName}] Config file is not needed.",
						pluginData.PluginInfo.Name);

					// if there is a config file, then add it to app configuration.
					if (configNeeded)
					{
						_logger
							.LogInformation(" [{pluginName}] Adding config file to application configuration...",
							pluginData.PluginInfo.Name);

						builder.AddJsonFile(configPath, false, true);
					}

					// register plugin to DI container.
					_logger
						.LogInformation(" [{pluginName}] Registering plugin to DI...",
						pluginData.PluginInfo.Name);

					pluginData.Registrator.RegisterPlugin(services, configuration);

					_logger
						.LogInformation(" [{pluginName}] Plugin successfully loaded.",
						pluginData.PluginInfo.Name);

					result.Add(plugin);
				}
				catch(Exception ex)
				{
					_logger
						.LogError(ex, " [{pluginName}] Plugin cannot be loaded.",
						pluginData.PluginInfo.Name);
				}
			}

			return result;
		}
	}
}
