using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Recording.Options;

namespace VoiceAssistant.Recording.Extensions
{
	public static class DIExtensions
	{
		public static IServiceCollection RegisterVoiceRecording(this IServiceCollection services,
			IConfiguration configuration)
		{
			services.AddTransient<CommandRecorder>();

			services.Configure<CommandRecorderOptions>(
				configuration.GetSection(nameof(CommandRecorderOptions)));

			return services;
		}
	}
}
