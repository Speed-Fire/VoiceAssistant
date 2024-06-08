using Microsoft.Extensions.DependencyInjection;
using Python.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAssistant.ChatGPT.Extensions
{
	internal class ChatGPTFactory
	{
		private readonly PyObject _module;

        public ChatGPTFactory()
        {
			using (Py.GIL())
			{
				_module = Py.Import("ChatGPT");
			}
        }

		public ChatGPT Create()
		{
			using (Py.GIL())
			{
				var instance = _module.InvokeMethod("CreateChatGpt");

				return new(instance);
			}
		}
    }

	public static class DIExtensions
	{
		public static IServiceCollection RegisterChatGPT(this IServiceCollection services)
		{
			services.AddSingleton<ChatGPTFactory>();

			services.AddTransient<ChatGPT>(provider =>
			{
				var factory = provider.GetRequiredService<ChatGPTFactory>();

				return factory.Create();
			});

			return services;
		}
	}
}
