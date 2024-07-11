using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Common;

namespace VoiceAssistant.CommandResolving.TextResolving.Resolvers
{
    internal interface ITextResolver<TResult> : IDisposable
    {
        bool IsInitialized { get; }
        Task<bool> Initialize();
        Task<OneOf<TResult, Exception>> Resolve(string text);
    }
}
