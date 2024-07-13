using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAssistant.Scripts.Models
{
	public sealed class UnderlyingScript(
		AssemblyLoadContext loadContext,
		MethodInfo method) 
		: IDisposable
	{
		private AssemblyLoadContext? _loadContext = loadContext;
		private MethodInfo? _method = method;

		private bool _disposed;

		public Task<bool> Execute(string[] args)
		{
			ObjectDisposedException.ThrowIf(_disposed, this);

			return (Task<bool>)_method!.Invoke(null, [args])!;
		}

		public void Dispose()
		{
			if(_disposed) 
				return;

			_loadContext!.Unload();
			_loadContext = null;
			_method = null;

			_disposed = true;
		}
	}
}
