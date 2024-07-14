using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using VoiceAssistant.Common;
using VoiceAssistant.Scripts.Compilation.Extensions;
using VoiceAssistant.Scripts.Compilation.Models;
using VoiceAssistant.Scripts.Interfaces;

namespace VoiceAssistant.Scripts.Compilation.Parsers
{
	internal class JsonToScriptBlocksParser(IScriptBlockService scriptBlockService)
	{
		private readonly IScriptBlockService _scriptBlockService = scriptBlockService;

		public OneOf<ScriptTree, Exception> Parse(string json)
		{
			try
			{
				var root = JsonNode
					.Parse(json, documentOptions: new() { AllowTrailingCommas = true })
					?.AsObject();

				if (root is null || root["Entry"] is null || root["Script"] is null)
					return new JsonException("Incorrect json!");

				var entry = GetEntry(root["Entry"]!.AsObject());

				var scriptBlocks = GetScriptBlockNodes(root["Script"]!.AsObject());

				return new ScriptTree(entry, scriptBlocks);
			}
			catch (Exception ex)
			{
				return ex;
			}
		}

		#region Entry node

		private static EntryNode GetEntry(JsonObject entryRoot)
		{
			var list = new List<Tuple<string, Type>>();

			foreach (var item in entryRoot)
			{
				var typename = item.Value!.GetValue<string>();
				var type = ResolveType(typename);
				if (type is null)
					throw new Exception($"{typename} is not a system type!");

				list.Add(Tuple.Create(item.Key, type));
			}

			return new EntryNode(list);
		}

		private static Type? ResolveType(string typeName)
		{
			var system = typeof(object).Assembly;

			var type = system.GetType(typeName);

			return type;
		}

		#endregion

		#region ScriptBlock nodes

		private List<ScriptBlockNode> GetScriptBlockNodes(JsonObject scriptRoot)
		{
			var result = new List<ScriptBlockNode>();

			var anType = new
			{
				Name = "",
				Parameters = new List<string>()
			};

			foreach(var item in scriptRoot)
			{
				var obj = item.Value!.DeserializeAnonymousType(anType);

				var node = GetScriptBlockNode(obj!.Name, obj.Parameters);

				result.Add(node);
			}

			return result;
		}

		private ScriptBlockNode GetScriptBlockNode(string name, List<string> parameters)
		{
			var scriptBlock = _scriptBlockService
				.ScriptBlocks
				.First(sb => sb.Name == name);

			return new(scriptBlock,
				parameters);
		}

		#endregion
	}
}
