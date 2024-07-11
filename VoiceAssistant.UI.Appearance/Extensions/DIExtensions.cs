using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Core.SettingsHelpers;
using VoiceAssistant.UI.Appearance.DictionarySelection;
using VoiceAssistant.UI.Appearance.Helpers;
using VoiceAssistant.UI.Appearance.Options;

namespace VoiceAssistant.UI.Appearance.Extensions
{
	public static class DIExtensions
	{
		public static IServiceCollection RegisterAppearance(
			this IServiceCollection services,
			IConfiguration config)
		{
			services
				.AddSingleton<LanguageSelector>()
				.AddSingleton<ThemeSelector>()
				.AddTransient<IAppearanceSettingsHelper, AppearanceSettingsHelper>();

			services.Configure<AppearanceOptions>(config.GetSection("Application:Appearance"));

			return services;
		}
	}
}
