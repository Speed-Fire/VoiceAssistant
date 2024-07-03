using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Plugin.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Core.Interfaces;
using VoiceAssistant.Core.Misc;
using VoiceAssistant.Core.Models;

namespace Plugin.Registrator
{
	public sealed class PluginRegistrator(string pluginFolderPath) : IDisposable
	{
		private readonly string _pluginFolderPath = pluginFolderPath;
		private readonly List<PluginLoadContext> _contextes = [];

		public IEnumerable<PluginInfo> Register(
			IServiceCollection services, 
			IConfiguration configuration)
		{
			var files = Directory.GetFiles(_pluginFolderPath, "*.dll");

			var pluginInfos = new List<PluginInfo>();

			foreach (var file in files)
			{
				var context = new PluginLoadContext(file);
				var assemblyName = new AssemblyName(Path.GetFileNameWithoutExtension(file));
				Assembly? assembly;

				// try to load assembly

				try
				{
					assembly = context.LoadFromAssemblyName(assemblyName);
				}
				catch
				{
					continue;
				}

				// try get plugin info.
				var pluginInfo = GetPluginInfo(assembly);

				// if there is no plugin info, go to next assembly
				if (pluginInfo is null)
				{
					context.Unload();
					continue;
				}

				// try register plugin
				var registered = TryRegisterPlugin(services, configuration, assembly);

				// if plugin can't be registered, go to next assembly.
				if (!registered)
				{
					context.Unload();
					continue;
				}

				// if plugin is successfully registered, add its info to provider, 
				//   save its assembly context
				//   and try to register plugin's config.

				var localConfigPath = TryCreatePluginDefaultConfig(assembly);

				TryLoadPluginConfig(configuration, localConfigPath);

				pluginInfos.Add(pluginInfo);

				_contextes.Add(context);
			}

			return pluginInfos;
		}

		private bool TryRegisterPlugin(
			IServiceCollection services,
			IConfiguration config,
			Assembly assembly)
		{
			// find an implementation of IDIPluginRegistrator.
			var pluginRegistretorType = assembly
				.GetTypes()
				.Where(t => t.IsAssignableTo(typeof(IDIPluginRegistrator)))
				.FirstOrDefault();

			// if not found, then return false.
			if (pluginRegistretorType is null)
				return false;

			IDIPluginRegistrator? instance;

			// try to create an instance of IDIPluginRegistrator.
			//  if it can't be created, then return false.
			try
			{
				instance = (IDIPluginRegistrator?)Activator.CreateInstance(pluginRegistretorType);
			}
			catch { return false; }

			if (instance is null)
				return false;

			instance.RegisterPlugin(services, config);

			return true;
		}

		private static PluginInfo? GetPluginInfo(Assembly assembly)
		{
			var pluginInfoType = assembly
				.GetTypes()
				.Where(t => t.IsAssignableTo(typeof(PluginInfo)))
				.FirstOrDefault();

			if (pluginInfoType is null)
				return null;

			try
			{
				return (PluginInfo?)Activator.CreateInstance(pluginInfoType);
			}
			catch
			{
				return null;
			}
		}

		private static string? TryCreatePluginDefaultConfig(Assembly assembly)
		{
			var assemblyName = assembly.GetName().Name;

			var configName = $"{assemblyName}.json";
			var config = assembly.GetManifestResourceStream($"{assemblyName}.DefaultConfig.json");
			if (config is null)
				return null;

			var localPath = Path.Combine("Config", configName);
			var globalPath = Path.Combine(Directory.GetCurrentDirectory(), localPath);
			if (File.Exists(globalPath))
				return globalPath;

			using var file = File.Create(globalPath);

			config.CopyTo(file);

			return localPath;
		}

		private static void TryLoadPluginConfig(IConfiguration configuration, string? configPath)
		{
			if (string.IsNullOrWhiteSpace(configPath))
				return;

			var builder = (IConfigurationBuilder)configuration;
			builder.AddJsonFile(configPath, false, true);
		}

		public void Dispose()
		{
			foreach(var context in _contextes)
			{
				context.Unload();
			}
		}
	}
}
