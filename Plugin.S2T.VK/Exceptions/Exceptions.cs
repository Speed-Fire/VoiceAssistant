using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.S2T.VK.Exceptions
{
    public class ServiceKeyNotSpecifiedException : Exception
    {
        public ServiceKeyNotSpecifiedException() :
            base("Service key is not specified!")
        {
            
        }
    }

	public class InternalServerErrorException : Exception
	{
        public InternalServerErrorException() :
            base("Internal server error!")
        {
            
        }
    }

    public class AudioLimitExceededException : Exception
    {
        public AudioLimitExceededException() :
            base("Total audio duration limit reached!")
        {
            
        }
    }

    public class TooLongAudioException : Exception
    {
        public TooLongAudioException() :
            base("Audio file is too big!")
        {
            
        }
    }

    public class AudioNotFoundException : Exception
    {
        public AudioNotFoundException() :
            base("Audio file not found!")
        {
            
        }
    }

    public class SpeechRecognitionException : Exception
    {
        public SpeechRecognitionException() :
            base("Speech recognition failed.")
        {
            
        }
    }

    public class AudioTranscodingException : Exception
    {
        public AudioTranscodingException() :
            base("Audio transcoding failed! Try to upload another audio format.")
        {
            
        }
    }
}
