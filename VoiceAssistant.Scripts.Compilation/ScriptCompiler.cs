using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Common;
using VoiceAssistant.Scripts.Compilation.Parsers;
using VoiceAssistant.Scripts.Interfaces;

namespace VoiceAssistant.Scripts.Compilation
{
	internal class ScriptCompiler(
		JsonToScriptBlocksParser jsonParser)
		: IScriptCompiler
	{
		private readonly JsonToScriptBlocksParser _jsonParser = jsonParser;
		private readonly CodeCompiler _codeCompiler = new();

		public Task<OneOf<MemoryStream, Exception>> Compile(string input)
		{
			return Task.Run(() => CompileInternal(input));
		}

		private OneOf<MemoryStream, Exception> CompileInternal(string input)
		{
			var treeResult = _jsonParser.Parse(input);
			if (!treeResult.IsFirst)
				return treeResult.Second;

			var codeResult = ScriptTreeToCodeParser.Parse(treeResult.First);
			if(!codeResult.IsFirst)
				return codeResult.Second;

			var assemblies = treeResult.First.Nodes
				.SelectMany(n => n.ScriptBlock.Assemblies)
				.DistinctBy(assem => assem.FullName);

			var stream = _codeCompiler.Compile(codeResult.First, assemblies);
			if (stream is null)
				return new Exception("Compile error!");

			return stream;
		}

		public void Dispose()
		{
			_codeCompiler.Dispose();
		}
	}
}
