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
using VoiceAssistant.Services.Misc.Interfaces;

namespace VoiceAssistant.Services.Initializers
{
    public class AssistantActionsProviderInitializer(
        Provider<List<AssistantAction>> actions,
        AppDbContext dbContext,
        ILogger<AssistantActionsProviderInitializer> logger)
        : ISequentialInitializer
    {
        private readonly Provider<List<AssistantAction>> _actions = actions;
        private readonly AppDbContext _dbContext = dbContext;
        private readonly ILogger _logger = logger;

        public async Task Initialize()
        {
            try
            {
                _logger.LogInformation("Starting AssistantActions provider initialization...");

                var actions = await _dbContext.Actions.ToListAsync();

                _dbContext.ChangeTracker.Clear();

                _actions.Value = actions;

                _logger.LogInformation("AssistantActions provider initialization finished.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot initialize AssistantAction provider!");
            }
        }
    }
}
