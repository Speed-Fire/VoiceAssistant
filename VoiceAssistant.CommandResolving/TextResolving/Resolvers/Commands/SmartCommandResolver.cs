using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.CommandResolving.Misc;
using VoiceAssistant.Core.Interfaces;
using VoiceAssistant.Common;
using VoiceAssistant.Domain.Models;
using VoiceAssistant.Domain.Underlying;

namespace VoiceAssistant.CommandResolving.TextResolving.Resolvers.Commands
{
    internal class SmartCommandResolver : ITextResolver<UnderlyingCommand>
    {
        private const string SYSTEM_MSG = "Hi. I'll send you a enumerated list of possible actions. Then i'm going to send you some sentences and you must send me back the number of the most similar action. If the sentence is not similar to any actions, then send -1. You should send only number.";

        private readonly Func<IChatGPT> _chatGPTFactory;
        private readonly Provider<List<UnderlyingCommand>> _commands;
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        private bool isInitialized = false;
        public bool IsInitialized => isInitialized;

        private IChatGPT? _chat;

        private DateTime? _lastHistoryClean;
        private IEnumerable<UnderlyingCommand> _enabledCommands;

        public SmartCommandResolver(Func<IChatGPT> chatGPTFactpry,
            Provider<List<UnderlyingCommand>> commands)
        {
            _chatGPTFactory = chatGPTFactpry;
            _commands = commands;

            _enabledCommands = _commands.Value is null ? [] : _commands.Value.Where(a => a.IsEnabled);

            _commands.PropertyChanged += ActionsProvider_Updated;
        }

        public async Task<bool> Initialize()
        {
            isInitialized = false;

            try
            {
                _chat = _chatGPTFactory.Invoke();
                if (_chat is null)
                    return false;

                await _chat.SendMessage(SYSTEM_MSG);
                
                await _chat.SendMessage(GetActionsList());

                isInitialized = true;
            }
            catch { }

            return isInitialized;
        }

        public async Task<OneOf<UnderlyingCommand, Exception>> Resolve(string command)
        {
            if (!isInitialized || _chat is null)
            {
                return new NotInitializedException(nameof(SmartCommandResolver));
            }

            try
            {
                await TryClearHistory();

                var response = await _chat.SendMessage(command);
                if (int.TryParse(response, out var number))
                {
                    if (number < 0)
                        return new UnrecognizedCommandException();
                    else
                        return _enabledCommands.ElementAt(number);
                }

                return new UnrecognizedResponseException();
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        private async Task TryClearHistory()
        {
            if (_lastHistoryClean is null)
            {
                _lastHistoryClean = DateTime.Now;
                return;
            }

            if (DateTime.Now - _lastHistoryClean < TimeSpan.FromHours(1))
                return;

            _lastHistoryClean = DateTime.Now;
            _chat?.ClearHistory();

            await Initialize();
        }

        private string GetActionsList()
        {
            var sb = new StringBuilder();

            var i = 0;
            foreach (var action in _enabledCommands)
            {
                sb.Append($"{i++}. ");
                sb.AppendLine(action.Command);
            }

            return sb.ToString();
        }

        private async void ActionsProvider_Updated(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            await _semaphore.WaitAsync();

            _enabledCommands = _commands.Value is null ? [] : _commands.Value.Where(a => a.IsEnabled);

            _lastHistoryClean = DateTime.Now.AddDays(-1);

            await TryClearHistory();

            _semaphore.Release();
        }

        public void Dispose()
        {
            _commands.PropertyChanged -= ActionsProvider_Updated;
            _semaphore.Dispose();
        }
    }
}
