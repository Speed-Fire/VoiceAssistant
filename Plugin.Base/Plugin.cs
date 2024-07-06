using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.Base
{
	public static class Plugin
	{
		/// <summary>
		/// Gets plugin name. Intended to use directly in plugin assembly.
		/// Don't use it in intermediate plugin libraries.
		/// </summary>
		/// <returns></returns>
		public static string GetName()
		{
			var assembly = Assembly.GetCallingAssembly();

			var name = assembly.GetName().Name ?? string.Empty;

			return name;
		}

		/// <summary>
		/// Appends plugin name to the beginning of configuration property path.
		/// Don't use it in intermediate plugin libraries.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public static string ConfigPath(string key)
		{
			var assembly = Assembly.GetCallingAssembly();
			var name = assembly.GetName().Name ?? string.Empty;

			return $"{name}{(string.IsNullOrEmpty(name) ? "" : ':')}{key}";
		}

		/// <summary>
		/// Gets plugin name.
		/// </summary>
		/// <param name="assembly">Plugin's assembly.</param>
		/// <returns></returns>
		public static string GetName(Assembly assembly)
		{
			var name = assembly.GetName().Name ?? string.Empty;

			return name;
		}

		/// <summary>
		/// Appends plugin name to the beginning of configuration property path.
		/// </summary>
		/// <param name="assembly">Plugin's assembly.</param>
		/// <param name="key"></param>
		/// <returns></returns>
		public static string ConfigPath(Assembly assembly, string key)
		{
			var name = assembly.GetName().Name ?? string.Empty;

			return $"{name}{(string.IsNullOrEmpty(name) ? "" : ':')}{key}";
		}
	}
}
