using DBConfiguration.Misc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Core.Models;

namespace DBConfiguration.Extensions
{
	public static class ModelBuilderExtensions
	{
		public static void BuildSettingsModels(this ModelBuilder builder)
		{
			builder.Entity<Settings>();
			builder.ApplyConfiguration(new SettingsConfiguration());
		}
	}
}
