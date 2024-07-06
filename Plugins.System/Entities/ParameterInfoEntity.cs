using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PluginsSystem.Models;

namespace PluginsSystem.Entities
{
	public class ParameterInfoEntity
	{
		public string Title { get; }
		public string ConfigKey { get; }
		public object DefaultValue { get; }

		public Type ParameterType { get; }

		private readonly System.Reflection.MethodBase? _parseMethod;

		public ParameterInfoEntity(ParameterInfo info, PluginInfo pluginInfo)
		{
			Title = info.Title;
			ConfigKey = $"{pluginInfo.AssemblyName}{(string.IsNullOrEmpty(pluginInfo.AssemblyName) ? "" : ':')}{info.ConfigKey}";

			ParameterType = info.Type switch
			{
				"int" =>      typeof(long),
				"uint" =>     typeof(ulong),
				"float" =>    typeof(double),
				"bool" =>     typeof(bool),
				"datetime" => typeof(DateTime),
				"string" =>   typeof(string),
				_ =>          typeof(string),
			};
			
			if (ParameterType != typeof(string))
			{
				_parseMethod = ParameterType!
					.GetMethod("Parse",
							System.Reflection.BindingFlags.Public |
							System.Reflection.BindingFlags.Static |
							System.Reflection.BindingFlags.FlattenHierarchy,
						[typeof(string)])
					?? throw new InvalidOperationException("Type must implement method Parse(string) or be string!");

				DefaultValue = _parseMethod.Invoke(null, [info.DefaultValue])!;
			}
			else
			{
				DefaultValue = info.DefaultValue;
			}
		}

		public object Parse(string str)
		{
			if (ParameterType == typeof(string))
				return str;

			return _parseMethod!.Invoke(null, [str])!;
		}
	}
}
