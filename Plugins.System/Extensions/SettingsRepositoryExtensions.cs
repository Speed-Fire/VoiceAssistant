using Plugin.Base;
using PluginsSystem.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PluginsSystem.Extensions
{
	internal static class SettingsRepositoryExtensions
	{
		public static string? GetValue(
			this SettingsRepository repository,
			ParameterInfoEntity parameterInfo)
		{
			var key = parameterInfo.ConfigKey;

			return repository.GetValueAsString(key);
		}

		public static void SetValue(
			this SettingsRepository repository,
			ParameterInfoEntity parameterInfo,
			string value)
		{
			var key = parameterInfo.ConfigKey;

			var nvalue = parameterInfo.Parse(value);

			repository.SetValue(key, nvalue);
		}

		public static void SetDefaultValue(
			this SettingsRepository repository,
			ParameterInfoEntity parameterInfo)
		{
			var key = parameterInfo.ConfigKey;
			var defaultValue = parameterInfo.DefaultValue;

			repository.SetValue(key, defaultValue);
		}
	}
}
