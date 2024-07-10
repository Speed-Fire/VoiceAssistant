using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAssistant.Recording.KeywordRecognition
{
	internal sealed class LoopKeywordRecognizer : IKeywordRecognizer
	{
		public event Action? KeywordRecognized;

		public void Start()
		{
			KeywordRecognized?.Invoke();
		}

		public void Stop()
		{
			
		}

		public void Dispose()
		{
			
		}
	}
}
