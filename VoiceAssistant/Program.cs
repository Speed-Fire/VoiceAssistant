using DBConfiguration.Extensions;
using DBConfiguration.Misc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Plugin.Registrator;
using Synergy.Core;
using Synergy.WPF.Common.Extensions;
using Synergy.WPF.Navigation.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.ActionManagement.Extensions;
using VoiceAssistant.ChatGPT.Extensions;
using VoiceAssistant.DAL.Extensions;
using VoiceAssistant.DAL.Providers;
using VoiceAssistant.Extensions;
using VoiceAssistant.Recording.Extensions;
using VoiceAssistant.Services;
using VoiceAssistant.Services.Extensions;

namespace VoiceAssistant
{
	internal class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = Host.CreateApplicationBuilder(args);

			builder.Services
				.RegisterCore()
				.RegisterSynergyWPFCommon()
				.RegisterSynergyWPFNavigation()
				.RegisterDAL(builder.Configuration)
				.RegisterVoiceRecording(builder.Configuration)
				.RegisterServices()
				.RegisterApp()
				.RegisterHttpClient()
				.RegisterChatGPT(builder.Configuration)
				.RegisterCommandResolving();

			await RegisterPlugins(builder);

			var host = builder.Build();

			var hostrun = host.RunAsync();

			await hostrun;
		}

		private static async Task RegisterPlugins(HostApplicationBuilder builder)
		{
			var pluginFolder = Path.Combine(Directory.GetCurrentDirectory(), "Plugins");
			using var context = new AppDbContext(
				builder.Configuration.GetConnectionString("MainDb") ?? string.Empty);

			var settingsInitializer = new DefaultDbConfigInitializer(context);

			using var registrator = new PluginRegistrator(pluginFolder,
				settingsInitializer);

			await registrator.Register(builder.Services, builder.Configuration);
		}
	}
}
