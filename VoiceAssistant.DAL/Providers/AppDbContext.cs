using DBConfiguration.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.DAL.Misc;
using VoiceAssistant.Domain.Models;

namespace VoiceAssistant.DAL.Providers
{
	public class AppDbContext : DbContext
	{
		public DbSet<AssistantAction> Actions => Set<AssistantAction>();
		public DbSet<AssistantScript> Scripts => Set<AssistantScript>();
		public DbSet<AssistantScriptAssembly> ScriptAssemblies => Set<AssistantScriptAssembly>();
		public DbSet<AssistantScriptConstruction> ScriptConstructions => Set<AssistantScriptConstruction>();

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) 
        {
			Database.EnsureCreated();
		}

        public AppDbContext(string connectionString) :
            base(new DbContextOptionsBuilder().UseSqlite(connectionString).Options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.BuildSettingsModels();

			modelBuilder.ApplyConfiguration(new AssistantScriptConfiguration());

			base.OnModelCreating(modelBuilder);
		}
	}
}
