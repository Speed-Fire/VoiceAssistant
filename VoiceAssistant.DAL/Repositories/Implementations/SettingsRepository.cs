using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Core.Models;

namespace VoiceAssistant.DAL.Repositories.Implementations
{
	internal class SettingsRepository(DbContext context) : IAsyncRepository<Settings>
	{
		private readonly DbContext _context = context;
		private readonly DbSet<Settings> _settingsSet = context.Set<Settings>();

		private volatile bool _notSaved = false;

		public async Task<Settings?> FindById(long id)
		{
			return await _settingsSet.FindAsync(id);
		}

		public IQueryable<Settings> GetAll()
		{
			return _settingsSet;
		}

		public async Task Add(Settings entity, bool autoSave = true)
		{
			await _settingsSet.AddAsync(entity);

			if(!autoSave)
			{
				_notSaved = true;
				return;
			}	

			await _context.SaveChangesAsync();
		}

		public async Task Delete(Settings entity, bool autoSave = true)
		{
			_settingsSet.Remove(entity);

			if (!autoSave)
			{
				_notSaved = true;
				return;
			}

			await _context.SaveChangesAsync();
		}

		public async Task Update(Settings entity, bool autoSave = true)
		{
			_settingsSet.Update(entity);

			if (!autoSave)
			{
				_notSaved = true;
				return;
			}

			await _context.SaveChangesAsync();
		}

		public async Task SaveChanges()
		{
			if (!_notSaved)
				return;

			await _context.SaveChangesAsync();
		}
	}
}
