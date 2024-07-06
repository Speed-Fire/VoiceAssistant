using PluginsSystem.Entities;
using Plugin.Base.Extensions;
using PluginsSystem.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using Plugin.Base;

namespace PluginsSystem.Configuration
{
	internal class ConfigGenerator(string configFolder)
	{
		private readonly string _configFolder = configFolder;

		/// <summary>
		/// Tries to create plugin configuration file.
		/// </summary>
		/// <param name="pluginInfo">Plugin to create config of.</param>
		/// <param name="path">Plugin configuration file path.</param>
		/// <returns>True if created or is already existing; False if plugin has no parameters.</returns>
		public bool TryCreateConfig(PluginInfoEntity pluginInfo, out string path)
		{
			path = GetConfigPath(pluginInfo);

			if(File.Exists(path))
				return true;

			if (pluginInfo.Parameters.Count == 0)
				return false;

			var root = new JsonObject();

			foreach (var param in pluginInfo.Parameters)
			{
				root.SetNestedValue(param.ConfigKey, param.DefaultValue);
			}

			using var output = File.OpenWrite(path);
			using var utf8wr = new Utf8JsonWriter(output);

			var json = root.ToJsonString(new()
			{
				WriteIndented = true,
				TypeInfoResolver = new DefaultJsonTypeInfoResolver()
			});

			utf8wr.WriteRawValue(json);

			return true;
		}

		public string GetConfigPath(PluginInfoEntity pluginInfo)
		{
			return Path.Combine(_configFolder, $"{pluginInfo.AssemblyName}.json");
		}
	}
}
