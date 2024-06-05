using IronPython.Hosting;
using IronPython.Modules;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CSPythonInvoker
{
	public class PEnvironment
	{
        private readonly ScriptEngine _engine;
        private readonly List<PScope> _scopes = [];

        public IReadOnlyList<PScope> Scopes => _scopes;

        public PEnvironment()
        {
            _engine = Python.CreateEngine();
        }

        public PScope CreateScope()
        {
            var scope = _engine.CreateScope();
            var pscope = new PScope(scope);

            _scopes.Add(pscope);

            return pscope;
        }

        public PScope CreateScopeWithDependencies(Assembly assembly, string resourceName)
        {
            var pscope = CreateScope();

			ArrayList metaPath = pscope._scope.GetVariable("meta_path");
			var importer = new ResourceMetaPathImporter(assembly,
				resourceName);
			metaPath.Add(importer);

			pscope._scope.SetVariable("meta_path", metaPath);

            return pscope;
		}
    }
}
