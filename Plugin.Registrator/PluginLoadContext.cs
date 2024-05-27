using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.Registrator
{
	internal class PluginLoadContext : AssemblyLoadContext
	{
		private readonly AssemblyDependencyResolver _resolver;

        public PluginLoadContext(string pluginPath) : base(true)
        {
            _resolver = new(pluginPath);
        }

		protected override Assembly? Load(AssemblyName assemblyName)
		{
			var assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
			if(assemblyPath is not null)
			{
				return LoadFromAssemblyPath(assemblyPath);
			}

			return null;
		}

		protected override nint LoadUnmanagedDll(string unmanagedDllName)
		{
			var assemblyPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
			if (assemblyPath is not null)
			{
				return LoadUnmanagedDllFromPath(assemblyPath);
			}

			return IntPtr.Zero;
		}
	}
}
