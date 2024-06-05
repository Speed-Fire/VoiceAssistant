using CSPythonInvoker;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAssistant.ChatGPT.Extensions
{
	public static class DIExtensions
	{
		private const string KEY = "VoiceAssistant.ChatGPT";

		public static IServiceCollection RegisterChatGPT(this IServiceCollection services)
		{
			services.AddKeyedSingleton(KEY, (provider, key) =>
			{
				// getting main script
				using var stream = Assembly.GetExecutingAssembly()
					.GetManifestResourceStream("VoiceAssistant.ChatGPT.Resources.ChatGPT.py");
				if (stream is null)
					throw new InvalidOperationException("Main script is not found!");
				using var reader = new StreamReader(stream);

				var code = reader.ReadToEnd();

				// getting python environment
				var env = provider.GetRequiredService<PEnvironment>();

				// creating scope with main script dependencies
				var scope = env.CreateScopeWithDependencies(
					Assembly.GetExecutingAssembly(),
					"VoiceAssistant.ChatGPT.Resources.scripts.zip");

				// loading main script into scope.
				scope.LoadScriptFromString(code);

				return scope;
			});

			services.AddTransient(provider =>
			{
				var scope = provider.GetRequiredKeyedService<PScope>(KEY);

				var instance = scope.CreateInstance("ChatGPT");				
				if (instance is null)
					throw new InvalidOperationException("Python class is not found!");

				return new ChatGPT(instance);
			});

			return services;
		}
	}
}
