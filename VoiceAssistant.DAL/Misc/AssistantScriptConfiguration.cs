using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VoiceAssistant.Domain.Models;

namespace VoiceAssistant.DAL.Misc
{
	internal class AssistantScriptConfiguration : IEntityTypeConfiguration<AssistantScript>
	{
		public void Configure(EntityTypeBuilder<AssistantScript> builder)
		{
			var serializerOptions = new JsonSerializerOptions();
			serializerOptions.Converters.Add(new JsonTypeConverter());

			builder
				.Property(script => script.Parameters)
				.HasConversion(
					p => JsonSerializer.Serialize(p, serializerOptions),
					p => JsonSerializer
						.Deserialize<List<Tuple<string, Type>>>(p, serializerOptions)!
				);
		}
	}
}
