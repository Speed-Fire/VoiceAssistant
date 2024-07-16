using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Common;
using VoiceAssistant.DAL.Providers;
using VoiceAssistant.Domain.Underlying;
using VoiceAssistant.Scripts.Interfaces;
using VoiceAssistant.Scripts.Models;
using VoiceAssistant.Services.Entities;
using VoiceAssistant.Services.Misc;

namespace VoiceAssistant.Services.UnderlyingCommands
{
	internal class UnderlyingCommandService(
		AppDbContext dbContext,
		Provider<List<UnderlyingCommand>> commands,
		IUnderlyingScriptFactory underlyingScriptFactory,
		[FromKeyedServices(ServiceConsts.UNDERLYING_COMMAND_EXECUTION)] 
			SemaphoreSlim underlyingExecutionSemaphore) 
		: IUnderlyingCommandService, IUnderlyingScriptService
	{
		private readonly AppDbContext _dbContext = dbContext;
		private readonly Provider<List<UnderlyingCommand>> _commands = commands;
		private readonly IUnderlyingScriptFactory _underlyingScriptFactory = underlyingScriptFactory;
		private readonly SemaphoreSlim _underlyingExecutionSemaphore = underlyingExecutionSemaphore;

		#region Update Commands

		public async Task<Exception?> AddAsync(AssistantCommandEntity entity)
		{
			await _underlyingExecutionSemaphore.WaitAsync();

			if (_commands.Value is null)
			{
				_underlyingExecutionSemaphore.Release();
				return null;
			}

			if (_commands.Value.FirstOrDefault(c => c.Id == entity.Id) is not null)
			{
				_underlyingExecutionSemaphore.Release();
				return new Exception("There is already command with the same Id!");
			}

			if (entity.AssistantScript is null ||
				entity.AssistantScript.AssistantScriptAssemblyId is null)
			{
				_underlyingExecutionSemaphore.Release();
				return new Exception("Script is not visible or not specified!");
			}

			var underlyingScriptResult =
				await LoadScript(entity.AssistantScript.AssistantScriptAssemblyId.Value);

			if (!underlyingScriptResult.IsFirst)
			{
				_underlyingExecutionSemaphore.Release();
				return underlyingScriptResult.Second;
			}

			var underlyingCommand = new UnderlyingCommand(entity.Id)
			{
				Command = entity.Command,
				Input = entity.Input,
				IsEnabledUser = entity.IsEnabled,
				NeedsConfirmation = entity.NeedsConfirmation,
				Script = underlyingScriptResult.First
			};

			_commands.Value.Add(underlyingCommand);
			_commands.Refresh();

			_underlyingExecutionSemaphore.Release();
			return null;
		}

		public async Task<Exception?> UpdateAsync(AssistantCommandEntity entity)
		{
			await _underlyingExecutionSemaphore.WaitAsync();

			if (entity.AssistantScript is null)
				return new InvalidOperationException("Command must have a script!");

			if (!TryGetExistingCommand(entity.Id, out var command))
			{
				_underlyingExecutionSemaphore.Release();
				return null;
			}

			_commands.Value!.Remove(command);
			
			if(command.Script is null || command.Script.Id != entity.AssistantScriptId)
			{
				if (entity.AssistantScript.AssistantScriptAssemblyId is null)
				{
					_underlyingExecutionSemaphore.Release();
					return new Exception("Set script is not visible for invocation!");
				}

				var newScriptResult = await LoadScript(entity.AssistantScript.AssistantScriptAssemblyId.Value);
				if (!newScriptResult.IsFirst)
				{
					_underlyingExecutionSemaphore.Release();
					return newScriptResult.Second;
				}
	
				TryDropScript(command.Script);
				command.Script = newScriptResult.First;
			}

			command.Command = entity.Command;
			command.Input = entity.Input;
			command.NeedsConfirmation = entity.NeedsConfirmation;
			command.IsEnabledUser = entity.IsEnabled;

			_commands.Value!.Add(command);
			_commands.Refresh();

			_underlyingExecutionSemaphore.Release();
			return null;
		}

		public Task<Exception?> DeleteAsync(AssistantCommandEntity entity)
		{
			_underlyingExecutionSemaphore.Wait();

			if (!TryGetExistingCommand(entity.Id, out var command))
			{
				_underlyingExecutionSemaphore.Release();
				return Task.FromResult<Exception?>(null);
			}

			_commands.Value!.Remove(command);
			TryDropScript(command.Script);

			_commands.Refresh();

			_underlyingExecutionSemaphore.Release();
			return Task.FromResult<Exception?>(null);
		}

		#endregion

		#region Update scripts

		public Task<bool> CanScriptBeFreeUpdated(UnderlyingScript script)
		{
			if (_commands.Value is null)
				return Task.FromResult(false);

			var result = _commands.Value
				.Where(x => x.Script.Id == script.Id)
				.Any();

			return Task.FromResult(!result);
		}

		public async Task<Exception?> UpdateScript(UnderlyingScript script)
		{
			await _underlyingExecutionSemaphore.WaitAsync();

			if (_commands.Value is null)
			{
				_underlyingExecutionSemaphore.Release();
				return null;
			}

			var commands = _commands.Value
				.Where(c => c.Script.Id == script.Id);

			if (!commands.Any())
			{
				_underlyingExecutionSemaphore.Release();
				return null;
			}

			var trans = await _dbContext.Database.BeginTransactionAsync();

			try
			{
				await _dbContext.Commands
					.Where(a => a.AssistantScriptId == script.Id)
					.ExecuteUpdateAsync(props =>
						props.SetProperty(a => a.AssistantScriptId, (long?)null)
							 .SetProperty(a => a.IsEnabled, false));

				await trans.CommitAsync();

				foreach (var command in commands)
					_commands.Value.Remove(command);

				_commands.Refresh();

				return null;
			}
			catch (Exception ex)
			{
				await trans.RollbackAsync();

				return ex;
			}
			finally
			{
				await trans.DisposeAsync();
				_underlyingExecutionSemaphore.Release();
			}
		}

		#endregion

		#region Internal

#nullable disable
		private bool TryGetExistingCommand(long id, out UnderlyingCommand command)
		{
			command = null;

			if (_commands.Value is null)
				return false;

			var existing = _commands.Value.FirstOrDefault(c => c.Id == id);

			if (existing is null)
				return false;

			command = existing;

			return true;
		}
#nullable enable

		private void TryDropScript(UnderlyingScript? script)
		{
			if (script is null)
				return;

			if (_commands.Value!.Where(c => c.Script == script).Count() > 1)
				return;

			script.Dispose();
		}

		private async Task<OneOf<UnderlyingScript, Exception>> LoadScript(long id)
		{
			var assembly = await _dbContext.ScriptAssemblies
				.FirstOrDefaultAsync(assem => assem.Id == id);

			if (assembly is null)
				return new Exception("Script assembly is not found!");

			_dbContext.ChangeTracker.Clear();

			var ms = new MemoryStream(assembly.RawAssembly);

			var underlyingResult = _underlyingScriptFactory.Create(ms, id);

			return underlyingResult;
		}

		#endregion
	}
}
