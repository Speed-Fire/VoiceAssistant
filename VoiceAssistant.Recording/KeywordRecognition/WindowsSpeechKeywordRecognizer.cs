using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAssistant.Recording.KeywordRecognition
{
	internal sealed class WindowsSpeechKeywordRecognizer : IKeywordRecognizer
	{
		private readonly SpeechRecognitionEngine _speechRecognitionEngine;

		public event Action? KeywordRecognized;

        public WindowsSpeechKeywordRecognizer(Stream grammarStream)
        {
			_speechRecognitionEngine =
				new SpeechRecognitionEngine(new System.Globalization.CultureInfo("en-US"));

			var grammar = new Grammar(grammarStream);

			_speechRecognitionEngine.LoadGrammar(grammar);
			_speechRecognitionEngine.SetInputToDefaultAudioDevice();

			_speechRecognitionEngine.SpeechRecognized += SpeechRecognized;
		}

		public void Start()
		{
			_speechRecognitionEngine.RecognizeAsync();
		}

		public void Stop()
		{
			_speechRecognitionEngine.RecognizeAsyncStop();
		}

		private void SpeechRecognized(object? sender, SpeechRecognizedEventArgs e)
		{
			KeywordRecognized?.Invoke();
		}

		public void Dispose()
		{
			_speechRecognitionEngine.Dispose();
		}
	}
}
