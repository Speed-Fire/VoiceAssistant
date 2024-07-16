using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.InteropServices;

namespace VoiceAssistant.Scripts.Compilation
{
    internal class CodeCompiler : IDisposable
    {
		private readonly IEnumerable<AssemblyMetadata> _assemblyMetadatas = LoadMetadatas();
		private readonly CSharpParseOptions _parseOptions = new(
				LanguageVersion.Latest,
				DocumentationMode.None,
				SourceCodeKind.Regular);
		private readonly CSharpCompilationOptions _compilationOptions = new(
				OutputKind.DynamicallyLinkedLibrary,
				optimizationLevel: OptimizationLevel.Release);

		public MemoryStream? Compile(string code, IEnumerable<Assembly> assemblies)
        {
			var metadataRefs = _assemblyMetadatas
				.Select(am => am.GetReference())
				.Concat(assemblies
						.Select(assem => MetadataReference.CreateFromFile(assem.Location)));
			var syntaxTree = CSharpSyntaxTree.ParseText(code, _parseOptions);
			
			var compilation = CSharpCompilation
				.Create(
					"ScriptAssembly",
					[syntaxTree],
					metadataRefs,
					_compilationOptions);

			var ms = new MemoryStream();
			var result = compilation.Emit(ms);

			if (!result.Success)
				return null;
			
			return ms;
		}

		public void Dispose()
		{
			foreach(var metadata in _assemblyMetadatas)
				metadata.Dispose();
		}

		private static List<AssemblyMetadata> LoadMetadatas()
		{
			var systemMetadata = AssemblyMetadata.CreateFromFile(typeof(object).Assembly.Location);

			var tmp = Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(), "System.Runtime.dll");
			var runtimeMetadata = AssemblyMetadata.CreateFromFile(tmp);

			return [systemMetadata, runtimeMetadata];
		}
	}
}
