using DBConfiguration.Providers;
using Microsoft.EntityFrameworkCore;

namespace DBConfiguration.Misc
{
    public class ConfigurationEntityLoadExceptionContext<TDbContext>
        where TDbContext : DbContext
    {
        public Exception Exception { get; }
        public bool Ignore { get; set; }
        public EntityConfigurationSource<TDbContext> Source { get; }

        public ConfigurationEntityLoadExceptionContext(
            EntityConfigurationSource<TDbContext> source,
            Exception exception)
        {
            Source = source;
            Exception = exception;
        }
    }
}
