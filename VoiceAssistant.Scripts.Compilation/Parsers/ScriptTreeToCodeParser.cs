using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Common;
using VoiceAssistant.Scripts.Compilation.Models;

namespace VoiceAssistant.Scripts.Compilation.Parsers
{
	internal class ScriptTreeToCodeParser
	{
		public static OneOf<string, Exception> Parse(ScriptTree tree)
		{
			try
			{
				var sb = new StringBuilder();

				AddNamespaces(sb, tree.Nodes);
				AddClassHeader(sb);
				AddGlobals(sb, tree.Nodes);
				AddFunctions(sb, tree.Nodes);
				AddMethodHeader(sb, tree.Entry,
					tree.Nodes.Select(n => n.ScriptBlock.IsAsync).Aggregate((v1, v2) => v1 || v2));
				AddMethodBody(sb, tree);
				AddMethodFooter(sb);
				AddClassFooter(sb);

				return sb.ToString();
			}
			catch(Exception ex)
			{
				return ex;
			}
		}

		private static void AddFunctions(StringBuilder sb, IReadOnlyList<ScriptBlockNode> nodes)
		{
			var sblocks = nodes.Select(n => n.ScriptBlock).Distinct();

			foreach(var sblock in sblocks)
			{
				sb.Append($"private  {(sblock.IsAsync ? "async Task" : "void")} {sblock.Name}Function(");

				foreach(var parameter in sblock.Parameters)
				{
					sb.Append($"{parameter.Item2.Name} {parameter.Item1},");
				}

				sb[^1] = ')';
				sb
					.Append('{')
					.Append(sblock.Code)
					.AppendLine("}");
			}
		}

		private static void AddMethodBody(StringBuilder sb, ScriptTree tree)
		{
			foreach(var node in tree.Nodes)
			{
				AddFunctionInvokation(sb, node);
			}
		}

		private static void AddFunctionInvokation(StringBuilder sb, ScriptBlockNode node)
		{
			if (node.InputValues.Count != node.ScriptBlock.Parameters.Count)
				throw new Exception("Incorrect count of input parameters!");

			if (node.ScriptBlock.IsAsync)
				sb.Append("await ");

			sb.Append($"{node.ScriptBlock.Name}Function(");

			for(int i = 0; i < node.InputValues.Count; i++)
			{
				var paramType = node.ScriptBlock.Parameters[i].Item2;
				var typeCode = Type.GetTypeCode(paramType);
				var input = node.InputValues[i];

				if (input.StartsWith("{Entry.") && input.EndsWith('}'))
				{
					var varname = input[7..^1];

					sb.Append($"{varname},");
				}
				else
				{
					if (typeCode == TypeCode.String)
						sb.Append($"\"{input}\",");
					else if (typeCode == TypeCode.DateTime || paramType == typeof(TimeSpan))
						sb.Append($"{paramType.Name}.Parse(\"{input}\"),");
					else
						sb.Append($"{input},");
				}
			}

			sb[^1] = ')';
			sb.AppendLine(";");
		}

		private static void AddMethodHeader(StringBuilder sb, EntryNode entry, bool isAsync)
		{
			sb
				.AppendLine($"public {(isAsync ? "async Task" : "void")} Execute(string[] args) {{");

			var itr = 0;
			foreach(var input in entry.EntryParamaters)
			{
				var rightSide =
					input.Item2 == typeof(string) ?
						$"args[{itr}]"
					:
						$"{input.Item2.Name}.Parse(args[{itr}])";

				itr++;

				sb
					.AppendLine($"var {input.Item1.Replace(' ', '_')}={rightSide};");
			}
		}

		private static void AddMethodFooter(StringBuilder sb)
		{
			sb.AppendLine("}");
		}

		private static void AddGlobals(StringBuilder sb, IReadOnlyList<ScriptBlockNode> nodes)
		{
			var globals = nodes
				.Select(n => n.ScriptBlock)
				.DistinctBy(n => n.Name)
				.SelectMany(n => n.GlobalVars)
				.Distinct();

			if (!globals.Any())
				return;

			// add fields
			foreach (var global in globals)
			{
				sb
					.Append("private ")
					.AppendLine(global);
			}

			// add constructor
			var paramList = new List<string>();
			sb.Append("public Script(");
			foreach (var global in globals)
			{
				var paramPair = global.Replace("_", "")[..(global.Length - 2)];
				paramList.Add(paramPair.Split(' ')[1]);

				sb
					.Append(paramPair)
					.Append(',');
			}
			sb[^1] = ')';

			sb.AppendLine("{");
			foreach(var param in paramList)
			{
				sb.AppendLine($"_{param}={param};");
			}
			sb.AppendLine("}");
		}

		private static void AddNamespaces(StringBuilder sb, IReadOnlyList<ScriptBlockNode> nodes)
		{
			var namespaces = nodes
				.Select(n => n.ScriptBlock)
				.DistinctBy(n => n.Name)
				.SelectMany(n => n.Namespaces)
				.Concat(["System"])
				.Distinct();

			foreach (var ns in namespaces)
			{
				sb
					.Append("using ")
					.Append(ns)
					.AppendLine(";");
			}
		}

		private static void AddClassHeader(StringBuilder sb)
		{
			sb
				.AppendLine("namespace Scripts {")
				.AppendLine("public class Script{");
		}

		private static void AddClassFooter(StringBuilder sb)
		{
			sb.AppendLine("}}");
		}
	}
}
