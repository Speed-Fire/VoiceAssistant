using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Core.Interfaces;
using VoiceAssistant.Common;
using VoiceAssistant.DAL.Providers;
using VoiceAssistant.Domain.Models;
using VoiceAssistant.Services.Entities;
using VoiceAssistant.Services.Extensions;
using VoiceAssistant.Services.Misc.Interfaces;
using VoiceAssistant.Services.UnderlyingCommands;

namespace VoiceAssistant.Services.AssistantCommands
{
	internal class AssistantCommandService(
		IUnderlyingCommandService underlyingCommandService,
		AppDbContext dbContext,
		ILogger<AssistantCommandService> logger)
		: IAssistantCommandService
	{
		private readonly IUnderlyingCommandService _underlyingCommandService = underlyingCommandService;
		private readonly AppDbContext _dbContext = dbContext;
		private readonly ILogger _logger = logger;

		#region GetAll

		public Task<IEnumerable<AssistantCommandEntity>> GetAllAsync()
		{
			return Task.Run(GetAllAsyncInternal);
		}

		private async Task<IEnumerable<AssistantCommandEntity>> GetAllAsyncInternal()
		{
			var result = await _dbContext.Commands.AsNoTracking().ToListAsync();

			return result.Select(a => a.Map()).ToList();
		}

		#endregion

		#region Create

		public Task<bool> CreateAsync(AssistantCommandEntity action)
		{
			return Task.Run(() => CreateAsyncInternal(action));
		}

		private async Task<bool> CreateAsyncInternal(AssistantCommandEntity action)
		{
			var trans = await _dbContext.Database.BeginTransactionAsync();

			try
			{
				AssistantCommand entity = action.Map();

				await _dbContext.Commands.AddAsync(entity);
				await _dbContext.SaveChangesAsync();
				await trans.CommitAsync();

				var res = await _underlyingCommandService.AddAsync(action);
				if (res is not null)
					_logger.LogWarning(res, 
						"AssistantCommand was added to db, but cannot be used in the application!");

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

				_dbContext.ChangeTracker.Clear();
			}
		}

		#endregion

		#region Update

		public Task<bool> UpdateAsync(AssistantCommandEntity action)
		{
			return Task.Run(() => UpdateAsyncInternal(action));
		}

		private async Task<bool> UpdateAsyncInternal(AssistantCommandEntity action)
		{
			var trans = await _dbContext.Database.BeginTransactionAsync();

			try
			{
				/// I am not sure, if _dbContext.ChangeTracker.Clear() clears
				///  _dbContext.Actions.Local as well.
				//var existing = _dbContext.Actions.Local.FirstOrDefault(a => a.Id == action.Id);
				//var entity = existing is null ? action.Map() : action.Map(existing);

				var entity = action.Map();

				_dbContext.Commands.Update(entity);
				await _dbContext.SaveChangesAsync();
				await trans.CommitAsync();

				var res = await _underlyingCommandService.UpdateAsync(action);
				if(res is not null)
					_logger.LogWarning(res,
						"AssistantCommand was updated in db, but cannot be used in the application!");

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

				_dbContext.ChangeTracker.Clear();
			}
		}

		#endregion

		#region Delete

		public Task<bool> DeleteAsync(AssistantCommandEntity action)
		{
			return Task.Run(() => DeleteAsyncInternal(action));
		}

		private async Task<bool> DeleteAsyncInternal(AssistantCommandEntity action)
		{
			var trans = await _dbContext.Database.BeginTransactionAsync();

			try
			{
				await _dbContext.Commands.Where(a => a.Id == action.Id)
					.ExecuteDeleteAsync();

				await trans.CommitAsync();

				var res = await _underlyingCommandService.DeleteAsync(action);
				if (res is not null)
					_logger.LogWarning(res,
						"AssistantCommand was deleted from db, but cannot be deleted from the application!");

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

	}
}
