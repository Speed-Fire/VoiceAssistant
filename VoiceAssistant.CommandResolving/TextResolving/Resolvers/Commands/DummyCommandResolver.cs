using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.CommandResolving.Misc;
using VoiceAssistant.Common;
using VoiceAssistant.Domain.Models;
using VoiceAssistant.Domain.Underlying;

namespace VoiceAssistant.CommandResolving.TextResolving.Resolvers.Commands
{
    internal sealed class DummyCommandResolver : ITextResolver<UnderlyingCommand>
    {
        private readonly Provider<List<UnderlyingCommand>> _actions;
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        public bool IsInitialized => true;

        private IEnumerable<UnderlyingCommand> _enabledActions;

        public DummyCommandResolver(Provider<List<UnderlyingCommand>> actions)
        {
            _actions = actions;
            _enabledActions = _actions.Value is null ? [] : _actions.Value.Where(a => a.IsEnabled);

            _actions.PropertyChanged += ActionsProvider_Updated;
        }

        public Task<bool> Initialize()
        {
            return Task.FromResult(true);
        }

        public Task<OneOf<UnderlyingCommand, Exception>> Resolve(string command)
        {
            OneOf<UnderlyingCommand, Exception> result;
            if (_actions.Value is null)
            {
                result = new InvalidOperationException("AssistantActions aren't loaded!");
                goto finish;
            }

            // TODO: использовать нейросеть для удаления лишних звуков (ну, ээээ, эм, ммм и т.д.)
            var action = _enabledActions.FirstOrDefault(x => string.Equals(x.Command, command,
                StringComparison.OrdinalIgnoreCase));

            if (action is null)
                result = new UnrecognizedCommandException();
            else
                result = action;

            finish:
            return Task.FromResult(result);
        }

        private void ActionsProvider_Updated(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            _semaphore.Wait();

            _enabledActions = _actions.Value is null ? [] : _actions.Value.Where(a => a.IsEnabled);

            _semaphore.Release();
        }

        public void Dispose()
        {
            _actions.PropertyChanged -= ActionsProvider_Updated;
            _semaphore.Dispose();
        }
    }
}
