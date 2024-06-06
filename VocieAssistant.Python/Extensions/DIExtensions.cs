using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VocieAssistant.Python.Extensions
{
	public static class DIExtensions
	{
		public static IServiceCollection RegisterPython(this IServiceCollection services)
		{
			services
				.AddSingleton<PythonInterop>();

			return services;
		}
	}
}
