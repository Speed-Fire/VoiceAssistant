using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Core.Interfaces;
using VoiceAssistant.Core.Models;

namespace DBConfiguration.Misc
{
	public class DefaultDbConfigInitializer(DbContext context) : IDefaultSettingsInitializer
	{
		private readonly DbContext _context = context;
		private readonly DbSet<Settings> _settingsSet = context.Set<Settings>();

		public async Task InitializeAsync(IEnumerable<Settings> defaultConfig)
		{
			foreach(var settings in defaultConfig)
			{
				if (await _settingsSet.AnyAsync(x => x.Id == settings.Id))
					continue;

				await _settingsSet.AddAsync(settings);
			}

			await _context.SaveChangesAsync();
		}

		public void Initialize(IEnumerable<Settings> defaultConfig)
		{
			foreach (var settings in defaultConfig)
			{
				if (_settingsSet.Any(x => x.Id == settings.Id))
					continue;

				_settingsSet.Add(settings);
			}

			_context.SaveChanges();
		}
	}
}
