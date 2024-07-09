using DBConfiguration.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Core.Models;
using VoiceAssistant.DAL.Providers;
using VoiceAssistant.DAL.Repositories;
using VoiceAssistant.DAL.Repositories.Implementations;

namespace VoiceAssistant.DAL.Extensions
{
	public static class DIExtensions
	{
		public static IServiceCollection RegisterDAL(this IServiceCollection services,
			IConfiguration configuration)
		{
			void SetupDbBuilder(DbContextOptionsBuilder contextBuilder)
			{
				var connectionString = configuration
						.GetConnectionString("MainDb") ?? string.Empty;

				contextBuilder.UseSqlite(connectionString);
			}

			services
				.AddDbContext<DbContext, AppDbContext>(SetupDbBuilder)
				.AddDbContext<AppDbContext>(SetupDbBuilder,
					contextLifetime: ServiceLifetime.Transient);

			services
				.AddTransient<IAsyncRepository<Settings>, SettingsRepository>();

			IConfigurationBuilder configBuilder = (IConfigurationBuilder)configuration;
			configBuilder.AddEntityConfiguration<AppDbContext>(SetupDbBuilder, true, 2000);

			return services;
		}
	}
}
