using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAssistant.Scripts.Models
{
	public abstract class ScriptBlockDefinition(
			string name,
			IReadOnlyList<Tuple<string, Type>> parameters)
	{
		public string Name { get; } = name;
		public IReadOnlyList<Tuple<string, Type>> Parameters { get; } = parameters;
	}

	public class ScriptBlock(
		string name,
		IReadOnlyList<Tuple<string, Type>> parameters,
		bool isAsync,
		bool canBeOptimizedToFunction,
		IEnumerable<Assembly> assemblies,
		IEnumerable<string> namespaces,
		string[] globalVars, 
		string code)
		: ScriptBlockDefinition(name, parameters)
	{
		public bool IsAsync { get; } = isAsync;
		public bool CanBeOptimizedToFunction { get; } = canBeOptimizedToFunction;
		public IEnumerable<Assembly> Assemblies { get; } = assemblies;
		public IEnumerable<string> Namespaces { get; } = namespaces;
		public string[] GlobalVars { get; } = globalVars;
		public string Code { get; } = code;
	}
}
