using Microsoft.Extensions.Options;
using NAudio.Utils;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Speech.Recognition;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using VoiceAssistant.Recording.Options;

namespace VoiceAssistant.Recording
{
	public class CommandRecorder : IDisposable
	{
		private const int MAX_WAVE_LENGTH_SECONDS = 30;

		public event Action<Stream>? CommandRecorded;

		private readonly SpeechRecognitionEngine _speechRecognitionEngine;
		private readonly AudioRecorder _audioRecorder;

		private volatile bool _disposing = false;

#nullable disable

		public CommandRecorder(IOptions<CommandRecorderOptions> options) : 
			this(Assembly.GetExecutingAssembly()
				.GetManifestResourceStream("VoiceAssistant.Recording.Grammars.Grammar_EN_US.xml"),
				options.Value)
		{

		}

#nullable enable

		public CommandRecorder(Stream stream, CommandRecorderOptions options)
		{
			_speechRecognitionEngine = 
				new SpeechRecognitionEngine(new System.Globalization.CultureInfo("en-US"));

			var grammar = new Grammar(stream);

			_speechRecognitionEngine.LoadGrammar(grammar);
			_speechRecognitionEngine.SetInputToDefaultAudioDevice();

			_speechRecognitionEngine.SpeechRecognized += Speech_Recognized;

			_audioRecorder = new(options.MaxSilenceDuration);
			_audioRecorder.Recorded += Audio_Recorded;
		}

		public void Start()
		{
			_speechRecognitionEngine.RecognizeAsync();
		}

		public void Stop()
		{
			_speechRecognitionEngine.RecognizeAsyncStop();
			_audioRecorder.Stop();
		}


		#region Speech recognized events

		private void Speech_Recognized(object? sender, SpeechRecognizedEventArgs e)
		{
			_speechRecognitionEngine.RecognizeAsyncStop();

			_audioRecorder.Start();
		}

		#endregion

		#region Audio recording events

		private void Audio_Recorded(Stream stream)
		{
			CommandRecorded?.Invoke(stream);

			if (!_disposing)
				_speechRecognitionEngine.RecognizeAsync();
		}

		#endregion

		public void Dispose()
		{
			_disposing = true;

			_audioRecorder.Dispose();

			_speechRecognitionEngine.RecognizeAsyncStop();
			_speechRecognitionEngine.Dispose();

		}
	}
}
