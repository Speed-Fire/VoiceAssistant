using NAudio.Utils;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAssistant.Recording
{
	internal class AudioRecorder : IDisposable
	{
		private const int MAX_WAVE_LENGTH_SECONDS = 30;
		private const short SILENCE_TREESHOLD = 500;

		private readonly int MAX_SILENCE_DURATION = 2;

		private readonly WaveInEvent _audioCapturer;
		private Stream? _output;
		private WaveFileWriter? _waveWriter;

		private volatile bool _speechRecording = false;
		private volatile int _silentChunkCount = 0;

		public event Action<Stream>? Recorded;

        public AudioRecorder(int silenceDuration)
        {
            _audioCapturer = new WaveInEvent();
			_audioCapturer.DataAvailable += Audio_DataAvailable;
			_audioCapturer.RecordingStopped += Audio_RecordingStopped;

			MAX_SILENCE_DURATION = silenceDuration;

			var format = _audioCapturer.WaveFormat;
		}

		public void Start()
		{
			_output = new MemoryStream();
			_waveWriter = new WaveFileWriter(new IgnoreDisposeStream(_output), _audioCapturer.WaveFormat);

			_silentChunkCount = 0;
			_speechRecording = true;
			_audioCapturer.StartRecording();
		}

		public void Stop()
		{
			_audioCapturer.StopRecording();
		}

		private readonly object _locker = new();

		private void Audio_RecordingStopped(object? sender, StoppedEventArgs e)
		{
			if (_speechRecording)
			{
				lock (_locker)
				{
					if (_speechRecording)
					{
						_speechRecording = false;

						_waveWriter?.Dispose();
						_waveWriter = null;

						if (_output is not null)
						{
							_output.Position = 0;
							Recorded?.Invoke(_output);
						}
					}
				}
			}
		}

		private async void Audio_DataAvailable(object? sender, WaveInEventArgs e)
		{
			if (_waveWriter is null)
				return;

			var task = _waveWriter.WriteAsync(e.Buffer, 0, e.BytesRecorded);

			if (IsSilent(e.Buffer))
			{
				_silentChunkCount++;
			}
			else
			{
				_silentChunkCount = 0;
			}

			Console.WriteLine(_silentChunkCount);

			await task;

			var maxSilentChunks = _audioCapturer.WaveFormat.AverageBytesPerSecond
				/ e.Buffer.Length * MAX_SILENCE_DURATION;

			if(_silentChunkCount > maxSilentChunks)
			{
				_audioCapturer.StopRecording();
			}

			if (_waveWriter.Position >
				_audioCapturer.WaveFormat.AverageBytesPerSecond * MAX_WAVE_LENGTH_SECONDS)
			{
				_audioCapturer.StopRecording();
			}
		}

		private unsafe bool IsSilent(ReadOnlySpan<byte> buffer)
		{
			Int16 max = Int16.MinValue;

			fixed (void* tmp = buffer)
			{
				Int16* buf = (Int16*)tmp;

				var length = buffer.Length / 2;
				for(int i = 0; i < length; i++)
				{
					if (buf[i] < 0)
					{
						max = Int16.Max(max, (Int16)(-buf[i]));
					}
					else
					{
						max = Int16.Max(max, buf[i]);
					}
				}
			}

			return max < SILENCE_TREESHOLD;
		}

		public void Dispose()
		{
			_audioCapturer.StopRecording();

			while(_speechRecording) { }

			_audioCapturer.Dispose();
		}
    }
}
