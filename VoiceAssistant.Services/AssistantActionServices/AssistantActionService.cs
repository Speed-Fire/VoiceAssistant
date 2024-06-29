using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Core.Interfaces;
using VoiceAssistant.Core.Misc;
using VoiceAssistant.DAL.Providers;
using VoiceAssistant.Domain.Models;
using VoiceAssistant.Services.Entities;
using VoiceAssistant.Services.Extensions;
using VoiceAssistant.Services.Misc.Interfaces;

namespace VoiceAssistant.Services.AssistantActionServices
{
	internal class AssistantActionService(
		Provider<List<AssistantAction>> actions,
		AppDbContext dbContext,
		ILogger<AssistantActionService> logger)
		: IAssistantActionService, IProviderInitializer
	{
		private readonly Provider<List<AssistantAction>> _actions = actions;
		private readonly AppDbContext _dbContext = dbContext;
		private readonly ILogger _logger = logger;

		#region GetAll

		public Task<IEnumerable<AssistantActionEntity>> GetAllAsync()
		{
			if (_actions.Value is null)
				return Task.FromResult<IEnumerable<AssistantActionEntity>>([]);

			return Task.FromResult(_actions.Value.Select(x => x.Map()));
		}

		#endregion

		#region Create

		public Task<bool> CreateAsync(AssistantActionEntity action)
		{
			return Task.Run(() => CreateAsyncInternal(action));
		}

		private async Task<bool> CreateAsyncInternal(AssistantActionEntity action)
		{
			if(_actions.Value is null)
				return false;

			var trans = await _dbContext.Database.BeginTransactionAsync();

			try
			{
				AssistantAction entity = action.Map();

				await _dbContext.Actions.AddAsync(entity);
				await _dbContext.SaveChangesAsync();
				await trans.CommitAsync();

				_dbContext.ChangeTracker.Clear();

				_actions.Value.Add(entity);

				RefreshProvider();

				return true;
			}
			catch(Exception ex)
			{
				await trans.RollbackAsync();

				_logger.LogError(ex, "Cannot create new AssistantAction!");

				return false;
			}
			finally
			{
				await trans.DisposeAsync();
			}
		}

		#endregion

		#region Update

		public Task<bool> UpdateAsync(AssistantActionEntity action)
		{
			return Task.Run(() => UpdateAsyncInternal(action));
		}

		private async Task<bool> UpdateAsyncInternal(AssistantActionEntity action)
		{
			if (_actions.Value is null)
				return false;

			var trans = await _dbContext.Database.BeginTransactionAsync();

			try
			{
				/// I am not sure, if _dbContext.ChangeTracker.Clear() clears
				///  _dbContext.Actions.Local as well.
				//var existing = _dbContext.Actions.Local.FirstOrDefault(a => a.Id == action.Id);
				//var entity = existing is null ? action.Map() : action.Map(existing);

				var entity = action.Map();

				_dbContext.Actions.Update(entity);
				await _dbContext.SaveChangesAsync();
				await trans.CommitAsync();

				_dbContext.ChangeTracker.Clear();

				var providerEntry = _actions.Value.First(a => a.Id == action.Id);
				var entryPos = _actions.Value.IndexOf(providerEntry);
				_actions.Value[entryPos] = entity;

				RefreshProvider();

				return true;
			}
			catch(Exception ex)
			{
				await trans.RollbackAsync();

				_logger.LogError(ex, "Cannot update AssistantAction!");

				return false;
			}
			finally
			{
				await trans.DisposeAsync();
			}
		}

		#endregion

		#region Delete

		public Task<bool> DeleteAsync(AssistantActionEntity action)
		{
			return Task.Run(() => DeleteAsyncInternal(action));
		}

		private async Task<bool> DeleteAsyncInternal(AssistantActionEntity action)
		{
			if (_actions.Value is null)
				return false;

			var trans = await _dbContext.Database.BeginTransactionAsync();

			try
			{
				await _dbContext.Actions.Where(a => a.Id == action.Id)
					.ExecuteDeleteAsync();

				await trans.CommitAsync();

				var providerEntry = _actions.Value.First(a => a.Id == action.Id);
				_actions.Value.Remove(providerEntry);

				RefreshProvider();

				return true;
			}
			catch (Exception ex)
			{
				await trans.RollbackAsync();

				_logger.LogError(ex, "Cannot delete AssistantAction!");

				return false;
			}
			finally
			{
				await trans.DisposeAsync();
			}
		}

		#endregion

		#region Initialize provider

		public async Task<bool> InitializeAsync()
		{
			try
			{
				var actions = await _dbContext.Actions.ToListAsync();

				_dbContext.ChangeTracker.Clear();

				_actions.Value = actions;

				return true;
			}
			catch(Exception ex)
			{
				_logger.LogError(ex, "Cannot initialize AssistantAction provider!");

				return false;
			}
		}

		#endregion

		private void RefreshProvider()
		{
			var tmp = new List<AssistantAction>(_actions.Value ?? []);
			_actions.Value = tmp;
		}
	}
}
