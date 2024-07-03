using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Plugin.Base;
using Plugin.S2T.Base;
using Plugin.S2T.VK.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Core.Interfaces;
using VoiceAssistant.Core.Models;

namespace Plugin.S2T.VK
{
    internal class DIS2TPluginVKRegistrator : IDIPluginRegistrator
	{
		private static readonly List<Settings> _defaultSettings = [
			new ("VK:ConverterSettings:ServiceApiKey", "", false)
		];

		public IEnumerable<Settings>? DefaultSettings => _defaultSettings;

		public void RegisterPlugin(IServiceCollection services, IConfiguration config)
		{
			RegisterConfiguration(config);
			RegisterServices(services, config);
		}

		private void RegisterConfiguration(IConfiguration config)
		{
			
		}

		private static void RegisterServices(IServiceCollection services,
			IConfiguration configuration)
		{
			services
				.Configure<VkConverterOptions>(
					configuration.GetSection("VK:ConverterSettings"));

			services
				.AddTransient<S2TConverterInfo, S2TConverterInfoVK>(provider => 
					{
						var converterFactory = () => provider.GetRequiredService<S2TConverterVK>();

						return new(converterFactory);
					})
				.AddTransient<S2TConverterVK>();
		}
	}
}
