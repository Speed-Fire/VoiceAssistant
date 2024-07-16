using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VoiceAssistant.DAL.Misc
{
	internal class JsonTypeConverter : JsonConverter<Type>
	{
		public override Type? Read(
			ref Utf8JsonReader reader,
			Type typeToConvert,
			JsonSerializerOptions options)
		{
			var mscorlibAssembly = typeof(object).Assembly;

			var type = mscorlibAssembly.GetType(reader.GetString()!);

			if (type is null)
				throw new Exception("Type is not found!");

			return type;
		}

		public override void Write(
			Utf8JsonWriter writer,
			Type value,
			JsonSerializerOptions options)
		{
			writer.WriteStringValue(value.FullName);
		}
	}
}
