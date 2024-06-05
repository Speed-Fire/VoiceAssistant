using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSPythonInvoker
{
	public class PScope
	{
		internal readonly ScriptScope _scope;

        private ScriptEngine Engine => _scope.Engine;

        private readonly List<ScriptSource> _sources = [];
        private readonly List<CompiledCode> _codes = [];

        internal PScope(ScriptScope scope)
        {
            _scope = scope;
        }

        public void LoadScriptFromFile(string filename)
        {
			var source = Engine.CreateScriptSourceFromFile(filename);
			var code = source.Compile();

			code.Execute(_scope);

			_sources.Add(source);
			_codes.Add(code);
		}

		public void LoadScriptFromString(string codeStr)
		{
			var source = Engine.CreateScriptSourceFromString(codeStr);
			var code = source.Compile();

			code.Execute(_scope);

            _sources.Add(source);
            _codes.Add(code);
		}

		public PInstance? CreateInstance(string classname)
        {
			var instance = Engine.Operations.Invoke(_scope.GetVariable(classname));
			if (instance is null)
				return null;
			
			return new(_scope, instance);
		}

        public TFunc? GetFunction<TFunc>(string name)
            where TFunc : Delegate
        {
            return _scope.GetVariable<TFunc>(name);
        }

		public void SetVariable(string varname, dynamic value)
		{
			_scope.SetVariable(varname, value);
		}

		public dynamic GetVariable(string varname)
		{
			return _scope.GetVariable(varname);
		}
	}
}
