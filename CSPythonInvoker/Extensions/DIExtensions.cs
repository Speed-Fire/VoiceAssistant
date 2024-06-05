using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSPythonInvoker.Extensions
{
	public static class DIExtensions
	{
		public static IServiceCollection RegisterPython(this IServiceCollection services)
		{
			services.AddSingleton<PEnvironment>();

			return services;
		}
	}
}
