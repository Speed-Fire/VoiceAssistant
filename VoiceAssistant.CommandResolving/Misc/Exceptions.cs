
using System.Globalization;
using VoiceAssistant.Common;

namespace VoiceAssistant.CommandResolving.Misc
{
    public class UnrecognizedCommandException : VoicableException
    {
        public UnrecognizedCommandException()
            : base(new()
            {
                ["ru-RU"] = "Команда не распознана.",
                ["en-US"]="Command is not recognized.",
                ["cs-CZ"]= "Příkaz je nerozpoznán"
			})
        {
        }
    }

    public class UnrecognizedResponseException : Exception
    {

    }

    public class NotInitializedException : Exception
    {
        public NotInitializedException(string obj) : base($"{obj} is not initialized!")
        {
            
        }
    }
}