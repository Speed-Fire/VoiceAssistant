using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAssistant.Domain.Underlying
{
	public sealed class UnderlyingScript(
		long id,
		AssemblyLoadContext loadContext,
		object instance,
		MethodInfo method) 
		: IDisposable
	{
		public long Id { get; } = id;

		private AssemblyLoadContext? _loadContext = loadContext;
		private object? _instance = instance;
		private MethodInfo? _method = method;

		private bool _disposed;

		public Task<bool> Execute(string[] args)
		{
			ObjectDisposedException.ThrowIf(_disposed, this);

			return (Task<bool>)_method!.Invoke(_instance, [args])!;
		}

		public void Dispose()
		{
			if(_disposed) 
				return;

			_loadContext!.Unload();
			_loadContext = null;
			_method = null;
			_instance = null;

			_disposed = true;
		}
	}
}
