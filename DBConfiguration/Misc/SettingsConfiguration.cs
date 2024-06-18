using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Core.Models;

namespace DBConfiguration.Misc
{
	public class SettingsConfiguration : IEntityTypeConfiguration<Settings>
	{
		public void Configure(EntityTypeBuilder<Settings> builder)
		{
			builder.ToTable("Settings");
			builder.HasKey(x => x.Id);
		}
	}
}
