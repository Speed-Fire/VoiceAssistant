using DBConfiguration.Misc;
using DBConfiguration.Providers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConfiguration.Extensions
{
	public static class ConfigurationBuilderExtensions
	{
		public static IConfigurationBuilder AddEntityConfiguration<TDbContext>(
			this IConfigurationBuilder builder,
			Action<DbContextOptionsBuilder> optionsAction,
			bool reloadOnChange = false,
			int pollingInterval = 5000,
			Action<ConfigurationEntityLoadExceptionContext<TDbContext>>? onLoadException = null)
			where TDbContext : DbContext
		{
			return builder.Add(
				new EntityConfigurationSource<TDbContext>(
					optionsAction,
					reloadOnChange,
					pollingInterval,
					onLoadException));
		}
	}
}
