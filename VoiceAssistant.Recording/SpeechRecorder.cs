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
using VoiceAssistant.Recording.KeywordRecognition;
using VoiceAssistant.Recording.Options;

namespace VoiceAssistant.Recording
{
	public enum SpeechRecognitionMode
	{
		OnKeyword,
		Loop
	}

	public sealed class SpeechRecorder : IDisposable
	{
		public event Action<Stream>? SpeechRecorded;

		private readonly Dictionary<SpeechRecognitionMode, IKeywordRecognizer> _keywordRecognizers;
		private readonly AudioRecorder _audioRecorder;
		private readonly object _lock = new();

		private IKeywordRecognizer _activeKeywordRecognizer;

		private volatile bool _recordingAudio = false;
		private volatile bool _disposing = false;

#nullable disable

		public SpeechRecorder(IOptions<SpeechRecorderOptions> options) : 
			this(Assembly.GetExecutingAssembly()
				.GetManifestResourceStream("VoiceAssistant.Recording.Grammars.Grammar_EN_US.xml"),
				options.Value)
		{

		}

#nullable enable

		public SpeechRecorder(Stream stream, SpeechRecorderOptions options)
		{
			_keywordRecognizers = InitializeKeywordRecognizersDictionary(stream);

			_activeKeywordRecognizer = _keywordRecognizers[SpeechRecognitionMode.OnKeyword];

			_audioRecorder = new(options.MaxSilenceDuration);
			_audioRecorder.Recorded += OnAudioRecorded;
		}

		public void Start()
		{
			_activeKeywordRecognizer.Start();
		}

		public void Stop()
		{
			_activeKeywordRecognizer.Stop();
			_audioRecorder.Stop();
		}

		public void SetMode(SpeechRecognitionMode mode)
		{
			_activeKeywordRecognizer.Stop();

			_activeKeywordRecognizer = _keywordRecognizers[mode];

			lock (_lock)
			{
				if (!_recordingAudio)
					_activeKeywordRecognizer.Start();
			}
		}

		#region Initialization

		private Dictionary<SpeechRecognitionMode, IKeywordRecognizer>
			InitializeKeywordRecognizersDictionary(Stream stream)
		{
			var normalKeywordRecognizer = new WindowsSpeechKeywordRecognizer(stream);
			normalKeywordRecognizer.KeywordRecognized += OnKeywordRecognized;

			var loopKeywordRecognizer = new LoopKeywordRecognizer();
			loopKeywordRecognizer.KeywordRecognized += OnKeywordRecognized;

			return new()
			{
				[SpeechRecognitionMode.OnKeyword] = normalKeywordRecognizer,
				[SpeechRecognitionMode.Loop] = loopKeywordRecognizer
			};
		}

		#endregion

		#region Speech recognized events

		private void OnKeywordRecognized()
		{
			lock (_lock)
			{
				_activeKeywordRecognizer.Stop();

				_audioRecorder.Start();

				_recordingAudio = true;
			}
		}

		#endregion

		#region Audio recording events

		private void OnAudioRecorded(Stream stream)
		{
			SpeechRecorded?.Invoke(stream);

			lock (_lock)
			{
				if (!_disposing)
					_activeKeywordRecognizer.Start();

				_recordingAudio = false;
			}
		}

		#endregion

		#region Dispose

		public void Dispose()
		{
			if (_disposing)
				return;

			_disposing = true;

			_audioRecorder.Dispose();

			_activeKeywordRecognizer.Stop();
			
			foreach(var keywordRecognizer in _keywordRecognizers.Values)
			{
				keywordRecognizer.Dispose();
			}
		}

		#endregion
	}
}
