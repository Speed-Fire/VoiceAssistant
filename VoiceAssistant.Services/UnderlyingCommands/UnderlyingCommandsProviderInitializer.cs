using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Common;
using VoiceAssistant.Core.Interfaces;
using VoiceAssistant.DAL.Providers;
using VoiceAssistant.Domain.Models;
using VoiceAssistant.Domain.Underlying;
using VoiceAssistant.Scripts.Interfaces;
using VoiceAssistant.Services.Misc.Interfaces;

namespace VoiceAssistant.Services.UnderlyingCommands
{
    public class UnderlyingCommandsProviderInitializer(
        IUnderlyingScriptFactory underlyingScriptFactory,
        Provider<List<UnderlyingCommand>> commands,
        AppDbContext dbContext,
        ILogger<UnderlyingCommandsProviderInitializer> logger)
        : ISequentialInitializer
    {
        private readonly IUnderlyingScriptFactory _underlyingScriptFactory = underlyingScriptFactory;
        private readonly Provider<List<UnderlyingCommand>> _commands = commands;
        private readonly AppDbContext _dbContext = dbContext;
        private readonly ILogger _logger = logger;

        public async Task Initialize()
        {
            try
            {
                _logger.LogInformation("Starting AssistantCommands provider initialization...");

                var commands = await _dbContext.Commands
                    .Include(c => c.AssistantScript).ThenInclude(s => s!.AssistantScriptAssembly)
                    .ToListAsync();

                _dbContext.ChangeTracker.Clear();

                var scriptDictionary = CreateUnderlyingScriptDictionary(commands);
                var underlyingCommands = new List<UnderlyingCommand>();

                foreach (AssistantCommand command in commands)
                {
                    UnderlyingCommand underlyingCommand = command;
                    if (command.AssistantScriptId is not null &&
                        scriptDictionary.TryGetValue(command.AssistantScriptId.Value, out var underlyingScript))
                    {
                        underlyingCommand.Script = underlyingScript;
                    }
                    underlyingCommands.Add(underlyingCommand);
                }

                _commands.Value = underlyingCommands;

                _logger.LogInformation("AssistantCommands provider initialization finished.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot initialize AssistantCommand provider!");
            }
        }

        private Dictionary<long, UnderlyingScript> CreateUnderlyingScriptDictionary(
            IEnumerable<AssistantCommand> commands)
        {
            var scripts = commands
                    .Select(c => c.AssistantScript)
                    .Where(x => x != null)
                    .Distinct()
                    .Where(x => x!.AssistantScriptAssembly is not null);

            var dictionary = new Dictionary<long, UnderlyingScript>();

            foreach (var script in scripts)
            {
                using var assemblyStream =
                    new MemoryStream(script!.AssistantScriptAssembly!.RawAssembly);

                var underlyingScriptResult = _underlyingScriptFactory
                        .Create(assemblyStream, script.Id);

                if (!underlyingScriptResult.IsFirst)
                {
                    _logger.LogWarning(underlyingScriptResult.Second,
                        "Assembly of script N{Id} cannot be loaded!", script.Id);
                }
                else
                {
                    dictionary[script.Id] = underlyingScriptResult.First;
                }
            }

            return dictionary;
        }
    }
}
