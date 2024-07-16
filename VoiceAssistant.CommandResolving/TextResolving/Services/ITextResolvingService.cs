using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Common;
using VoiceAssistant.Domain.Models;
using VoiceAssistant.Domain.Underlying;

namespace VoiceAssistant.CommandResolving.TextResolving.Services
{
    public interface ITextResolvingService : IDisposable
    {
        Task<bool> Initialize();

        Task<OneOf<bool?, Exception>> ResolveConfirmation(string text);
        Task<OneOf<UnderlyingCommand, Exception>> ResolveCommand(string text);
    }
}
