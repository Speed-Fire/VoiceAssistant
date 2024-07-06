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
using VoiceAssistant.Common;
using VoiceAssistant.Core.Models;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.IO;
using PluginsSystem.Models;
using Microsoft.Extensions.Logging;

namespace PluginsSystem.Examining
{
    internal sealed class PluginExaminer(
        string pluginFolderPath,
        ILogger<PluginExaminer> logger)
        : IDisposable
    {
        private class Root
        {
#nullable disable
            public PluginInfo PluginInfo { get; set; }
#nullable enable
        }

        private readonly ILogger _logger = logger;
        private readonly string _pluginFolderPath = pluginFolderPath;
        private readonly List<PluginLoadContext> _contextes = [];

        public IEnumerable<PluginData> Examine()
        {
            var files = Directory.GetFiles(_pluginFolderPath, "*.dll");

            var examinationData = new List<PluginData>();

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

				// try get plugin registrator
				var registrator = TryGetPluginRegistrator(assembly);

				// if plugin can't be registered, go to next assembly.
				if (registrator is null)
				{
					context.Unload();
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

                // if plugin is successfully registered, add its info to provider, 
                //   save its assembly context.

                examinationData.Add(new(pluginInfo, registrator));

                _contextes.Add(context);
            }

            return examinationData;
        }

        private IDIPluginRegistrator? TryGetPluginRegistrator(Assembly assembly)
        {
            // find an implementation of IDIPluginRegistrator.
            var pluginRegistretorType = assembly
                .GetTypes()
                .Where(t => t.IsAssignableTo(typeof(IDIPluginRegistrator)))
                .FirstOrDefault();

            // if not found, then return false.
            if (pluginRegistretorType is null)
                return null;

            IDIPluginRegistrator? instance;

            // try to create an instance of IDIPluginRegistrator.
            try
            {
                instance = (IDIPluginRegistrator?)Activator.CreateInstance(pluginRegistretorType);
            }
            catch(Exception ex)
            {
                var assemblyName = assembly.GetName().Name;

				_logger
                    .LogError(ex, "Cannot load plugin from assembly {assemblyName}: instance of {pluginRegistretor} cannot be created.",
                    assemblyName,
                    nameof(IDIPluginRegistrator));

				return null;
            }

            return instance;
        }

        private PluginInfo? GetPluginInfo(Assembly assembly)
        {
            var assemblyName = assembly.GetName().Name;

            if (assemblyName is null)
                return null;

            // get plugin info
            using var pluginInfoStream = assembly
                .GetManifestResourceStream($"{assemblyName}.PluginInfo.json");

            if (pluginInfoStream is null)
            {
				_logger
                    .LogError("Cannot load plugin from assembly {assemblyName}: there is no file \"PluginInfo.json\".", assemblyName);


				return null;
            }

            PluginInfo? pluginInfo;

            try
            {
                var root = JsonSerializer.Deserialize<Root>(pluginInfoStream);

                pluginInfo = root!.PluginInfo;
            }
            catch (Exception ex)
            {
                _logger
                    .LogError(ex, "Cannot load plugin from assembly {assemblyName}: file \"PluginInfo.json\" is incorrect.", assemblyName);

                return null;
            }

            if (pluginInfo is null)
                return null;

            // set assembly name
            pluginInfo.AssemblyName = assemblyName;

            // get plugin image
            var imageStream = assembly
                .GetManifestResourceStream($"{assemblyName}.Resources.image.png");

            pluginInfo.ImageStream = imageStream;

            // get plugin settings
            var settingsStream = assembly
                .GetManifestResourceStream($"{assemblyName}.Settings.xml");

            pluginInfo.SettingsStream = settingsStream;

            return pluginInfo;
        }

        public void Dispose()
        {
            foreach (var context in _contextes)
            {
                context.Unload();
            }
        }
    }
}
