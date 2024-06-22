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
using VoiceAssistant.Core.Models;

namespace Plugin.Registrator
{
	public class PluginRegistrator(string pluginFolderPath,
		IDefaultSettingsInitializer settingsInitializer) : IDisposable
	{
		private readonly string _pluginFolderPath = pluginFolderPath;
		private readonly IDefaultSettingsInitializer _settingsInitializer = settingsInitializer;
		private readonly List<PluginLoadContext> _contextes = [];

		public async Task<int> Register(IServiceCollection services, IConfiguration config)
		{
			var files = Directory.GetFiles(_pluginFolderPath, "*.dll");

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

				// find all implementations of IDIPluginRegistrator.
				var implementations = assembly
					.GetTypes()
					.Where(t => t.IsAssignableTo(typeof(IDIPluginRegistrator)));

				// if not found, then unload this context and go next.
				if (!implementations.Any())
				{
					context.Unload();
					continue;
				}

				// use all implementations to register plugin.
				var count = 0;
				foreach(var implementation in implementations)
				{
					IDIPluginRegistrator? instance;

					// try to create an instance of IDIPluginRegistrator.
					//  if it can't be created, then try next implementation.
					try
					{
						instance = (IDIPluginRegistrator?)Activator.CreateInstance(implementation);
					}
					catch { continue; }

					if (instance is null)
						continue;

					count++;
					instance.RegisterPlugin(services, config);
					if (instance.DefaultSettings is null)
						continue;

					await _settingsInitializer.InitializeAsync(instance.DefaultSettings);
				}

				// if no implementation has been registered, then unload this context and go next.
				if(count == 0)
				{
					context.Unload();
					continue;
				}

				_contextes.Add(context);
			}

			return _contextes.Count;
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
