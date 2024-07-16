using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Common;
using VoiceAssistant.Domain.Underlying;
using VoiceAssistant.Scripts.Interfaces;
using VoiceAssistant.Scripts.Models;

namespace VoiceAssistant.Scripts.Compilation
{
	internal class UnderlyingScriptFactory(IServiceProvider serviceProvider)
		: IUnderlyingScriptFactory
	{
		private readonly IServiceProvider _serviceProvider = serviceProvider;

		public OneOf<UnderlyingScript, Exception> Create(Stream stream, long id, bool leaveOpen = false)
		{
			stream.Position = 0;

			try
			{
				var context = new AssemblyLoadContext(null, true);
				var assembly = context.LoadFromStream(stream);

				var instanceType = assembly.GetType("Scripts.Script");
				if (instanceType is null)
					return new Exception("Type is not found!");

				var method = instanceType.GetMethod("Invoke",
					System.Reflection.BindingFlags.Public,
					[typeof(string[])]);
				if (method is null)
					return new Exception("Type has no invocation method!");
				
				var instance = ActivatorUtilities.CreateInstance(_serviceProvider, instanceType);

				if(!leaveOpen)
					stream.Dispose();

				return new UnderlyingScript(id, context, instance, method);
			}
			catch (Exception ex)
			{
				return ex;
			}
		}
	}
}
