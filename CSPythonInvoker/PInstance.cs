using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSPythonInvoker
{
	public class PInstance
	{
		private readonly ScriptScope _scope;
		private readonly object _instance;

		private ScriptEngine Engine => _scope.Engine;

		internal PInstance(ScriptScope scope, object instance)
		{
			_scope = scope;
			_instance = instance;
		}

		public void SetMember(string varname, dynamic value)
		{
			Engine.Operations.SetMember(_instance, varname, value);
		}

		public void SetMember<T>(string varname, T value)
		{
			Engine.Operations.SetMember(_instance, varname, value);
		}

		public dynamic GetMember(string varname)
		{
			return Engine.Operations.GetMember(_instance, varname);
		}

		public T GetMember<T>(string varname)
		{
			return Engine.Operations.GetMember<T>(_instance, varname);
		}

		public void CallMethod(string method, params dynamic[] arguments)
		{
			Engine.Operations.InvokeMember(_instance, method, arguments);
		}

		public dynamic CallFunction(string method, params dynamic[] arguments)
		{
			return Engine.Operations.InvokeMember(_instance, method, arguments);
		}
	}
}
