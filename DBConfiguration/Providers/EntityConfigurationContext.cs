using DBConfiguration.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConfiguration.Providers
{
	public sealed class EntityConfigurationContext(string connectionString) : DbContext
	{
		public DbSet<Settings> Settings => Set<Settings>();

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlite(connectionString);
		}
	}
}
