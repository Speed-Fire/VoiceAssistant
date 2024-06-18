using DBConfiguration.Misc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConfiguration.Providers
{

    public class EntityConfigurationSource<TDbContext> : IConfigurationSource
		where TDbContext : DbContext
	{
		public readonly Action<DbContextOptionsBuilder> OptionsAction;
		public readonly bool ReloadOnChange;
		public readonly int PollingInterval;
		public readonly Action<ConfigurationEntityLoadExceptionContext<TDbContext>>? OnLoadException;

		public EntityConfigurationSource(
			Action<DbContextOptionsBuilder> optionsAction,
			bool reloadOnChange = false,
			int pollingInterval = 5000,
			Action<ConfigurationEntityLoadExceptionContext<TDbContext>>? onLoadException = null)
		{
			if(pollingInterval < 500)
			{
				throw new ArgumentException($"{nameof(pollingInterval)} can't be less than 500",
					nameof(pollingInterval));
			}

			OptionsAction = optionsAction;
			ReloadOnChange = reloadOnChange;
			PollingInterval = pollingInterval;
			OnLoadException = onLoadException;
		}

		public IConfigurationProvider Build(IConfigurationBuilder builder)
		{
			return new EntityConfigurationProvider<TDbContext>(this);
		}
	}
}
