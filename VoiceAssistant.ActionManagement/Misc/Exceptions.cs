
using System.Globalization;
using VoiceAssistant.Core.Misc;

namespace VoiceAssistant.ActionManagement.Misc
{
    public class UnrecognizedCommandException : VoicableException
    {
        public UnrecognizedCommandException()
            : base(CultureInfo.GetCultureInfo("ru-ru"), "Команда не распознана.")
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