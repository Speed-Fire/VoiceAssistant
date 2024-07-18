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

		public Task<IEnumerable<AssistantCommand>> GetAllAsync()
		{
			return Task.Run(GetAllAsyncInternal);
		}

		private async Task<IEnumerable<AssistantCommand>> GetAllAsyncInternal()
		{
			var result = await _dbContext.Commands.AsNoTracking().ToListAsync();

			return result;
		}

		#endregion

		#region Create

		public Task<bool> CreateAsync(AssistantCommand command)
		{
			return Task.Run(() => CreateAsyncInternal(command));
		}

		private async Task<bool> CreateAsyncInternal(AssistantCommand command)
		{
			var trans = await _dbContext.Database.BeginTransactionAsync();

			try
			{
				await _dbContext.Commands.AddAsync(command);
				await _dbContext.SaveChangesAsync();
				await trans.CommitAsync();

				_dbContext.ChangeTracker.Clear();

				var res = await _underlyingCommandService.AddAsync(command);
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
			}
		}

		#endregion

		#region Update

		public Task<bool> UpdateAsync(AssistantCommand command)
		{
			return Task.Run(() => UpdateAsyncInternal(command));
		}

		private async Task<bool> UpdateAsyncInternal(AssistantCommand command)
		{
			var trans = await _dbContext.Database.BeginTransactionAsync();

			try
			{
				/// I am not sure, if _dbContext.ChangeTracker.Clear() clears
				///  _dbContext.Actions.Local as well.
				//var existing = _dbContext.Actions.Local.FirstOrDefault(a => a.Id == action.Id);
				//var entity = existing is null ? action.Map() : action.Map(existing);

				_dbContext.Commands.Update(command);
				await _dbContext.SaveChangesAsync();
				await trans.CommitAsync();

				_dbContext.ChangeTracker.Clear();

				var res = await _underlyingCommandService.UpdateAsync(command);
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
			}
		}

		#endregion

		#region Delete

		public Task<bool> DeleteAsync(AssistantCommand command)
		{
			return Task.Run(() => DeleteAsyncInternal(command));
		}

		private async Task<bool> DeleteAsyncInternal(AssistantCommand command)
		{
			var trans = await _dbContext.Database.BeginTransactionAsync();

			try
			{
				await _dbContext.Commands.Where(a => a.Id == command.Id)
					.ExecuteDeleteAsync();

				await trans.CommitAsync();

				var res = await _underlyingCommandService.DeleteAsync(command);
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
