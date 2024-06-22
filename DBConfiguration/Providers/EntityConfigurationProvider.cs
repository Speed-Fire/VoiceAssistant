using Configuration.EFCore;
using DBConfiguration.Misc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System.Text;
using VoiceAssistant.Core.Models;

namespace DBConfiguration.Providers
{
	public class EntityConfigurationProvider<TDbContext> : ConfigurationProvider, IDisposable
		where TDbContext : DbContext
	{
		private readonly EntityConfigurationSource<TDbContext> _configurationSource;
		private readonly CancellationTokenSource _cancellationTokenSource;
		private byte[] _lastComputedHash;
		private Task? _watchDbTask;
		private bool _disposed;

		public EntityConfigurationProvider(EntityConfigurationSource<TDbContext> configurationSource)
		{
			_configurationSource = configurationSource;
			_cancellationTokenSource = new();
			_lastComputedHash = new byte[20];
		}

		public override void Load()
		{
			if (_watchDbTask is not null)
				return;

			try
			{
				Data = GetData();
				_lastComputedHash = EntityConfigurationProvider<TDbContext>.ComputeHash(Data);
			}
			catch (Exception ex)
			{
				var exceptionContext = new ConfigurationEntityLoadExceptionContext<TDbContext>(
					_configurationSource,
					ex);

				_configurationSource.OnLoadException?.Invoke(exceptionContext);

				if (!exceptionContext.Ignore)
				{
					throw;
				}
			}

			var cancellationToken = _cancellationTokenSource.Token;
			if(_configurationSource.ReloadOnChange)
			{
				_watchDbTask = Task.Run(() => WatchDatabase(cancellationToken), 
					cancellationToken);
			}
		}

		private async Task WatchDatabase(CancellationToken cancellationToken)
		{
			while(!cancellationToken.IsCancellationRequested)
			{
				try
				{
					await Task.Delay(_configurationSource.PollingInterval, cancellationToken);

					var actualData = GetData();
					var hash = EntityConfigurationProvider<TDbContext>.ComputeHash(actualData);

					if (!hash.SequenceEqual(_lastComputedHash))
					{
						Data = actualData;
						OnReload();
					}
				}
				catch(Exception ex)
				{
					var exceptionContext = new ConfigurationEntityLoadExceptionContext<TDbContext>(
						_configurationSource,
						ex);

					_configurationSource.OnLoadException?.Invoke(exceptionContext);

					if (!exceptionContext.Ignore)
					{
						throw;
					}
				}
			}
		}

		private static byte[] ComputeHash(IDictionary<string, string?> dict)
		{
			var byteDict = new List<byte>();
			foreach(var pair in dict)
			{
				byteDict.AddRange(Encoding.UTF8.GetBytes(pair.Key + pair.Value));
			}

			return System.Security.Cryptography.SHA1.HashData(byteDict.ToArray());
		}

		private IDictionary<string, string?> GetData()
		{
			using var context = CreateDbContext();
			IQueryable<Settings> settings = EntityConfigurationProvider<TDbContext>.GetSettings(context);

			var settingsDictionary = settings.Any() ?
				settings.ToDictionary(c => c.Id, c => c.Value) :
				[];

			return EntityConfigurationProvider<TDbContext>.BuildSettingsDictionary(settingsDictionary);
		}

		private static IDictionary<string, string?> BuildSettingsDictionary(
			Dictionary<string, string> settingsDictionary)
		{
			var sb = new StringBuilder();
			sb.Append('{');

			foreach(var pair in settingsDictionary)
			{
				if (pair.Value.StartsWith('{') || pair.Value.StartsWith('['))
					sb.Append($"\"{pair.Key}\": {pair.Value}, ");
				else
					sb.Append($"\"{pair.Key}\": \"{pair.Value}\", ");
			}

			sb.Append('}');
			return JsonConfigurationParser.Parse(sb.ToString());
		}

		private static DbSet<Settings> GetSettings(TDbContext context)
		{
			DbSet<Settings> settings = context.Set<Settings>();

			return settings;
		}

		private TDbContext CreateDbContext()
		{
			var builder = new DbContextOptionsBuilder<TDbContext>();
			_configurationSource.OptionsAction(builder);

#pragma warning disable CS8600 // Преобразование литерала, допускающего значение NULL или возможного значения NULL в тип, не допускающий значение NULL.
#pragma warning disable CS8603 // Возможно, возврат ссылки, допускающей значение NULL.
			return (TDbContext)Activator.CreateInstance(typeof(TDbContext),
				[builder.Options]);
#pragma warning restore CS8603 // Возможно, возврат ссылки, допускающей значение NULL.
#pragma warning restore CS8600 // Преобразование литерала, допускающего значение NULL или возможного значения NULL в тип, не допускающий значение NULL.
		}

		public void Dispose()
		{
			if(_disposed) return;

			_cancellationTokenSource.Cancel();
			_cancellationTokenSource.Dispose();
			_disposed = true;
		}
	}
}