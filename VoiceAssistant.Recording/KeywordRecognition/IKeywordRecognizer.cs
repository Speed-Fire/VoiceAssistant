using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAssistant.Recording.KeywordRecognition
{
	internal interface IKeywordRecognizer : IDisposable
	{
		void Start();
		void Stop();

		event Action? KeywordRecognized;
	}
}
