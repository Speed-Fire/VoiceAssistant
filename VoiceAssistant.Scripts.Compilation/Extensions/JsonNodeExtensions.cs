using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Threading.Tasks;

namespace VoiceAssistant.Scripts.Compilation.Extensions
{
	internal static class JsonNodeExtensions
	{
		public static T? DeserializeAnonymousType<T>(
			this JsonNode node,
			T anonymousObject,
			JsonSerializerOptions? serializerOptions = default)
		{
			return node.Deserialize<T>(serializerOptions);
		}
	}
}
