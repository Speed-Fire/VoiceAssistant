using Microsoft.Extensions.Hosting;
using Plugin.S2T.Base;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Core.Interfaces;
using VoiceAssistant.Core.Misc;
using VoiceAssistant.Recording;

namespace VoiceAssistant.Services
{
	public class VoiceAssistantService : BackgroundService
	{
		private readonly CommandRecorder _commandRecorder;
		private readonly ConcurrentQueue<string> _actionsQueue;
		private readonly Provider<S2TConverterInfo> _S2TConverterProvider;
		private readonly IExceptionNotifier _exceptionNotifier;

		private readonly object _S2TLock = new();

		private IS2TConverter? S2TConverter { get; set; }

		public VoiceAssistantService(
			CommandRecorder commandRecorder,
			Provider<S2TConverterInfo> s2TConverterProvider,
			IExceptionNotifier exceptionNotifier)
		{
			_commandRecorder = commandRecorder;
			_commandRecorder.CommandRecorded += CommandRecorded;

			_actionsQueue = new();
			_S2TConverterProvider = s2TConverterProvider;
			_S2TConverterProvider.PropertyChanged += S2TConverterProvider_PropertyChanged;

			_exceptionNotifier = exceptionNotifier;
		}

		protected override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			return Task.Run(() =>
			{
				_commandRecorder.Start();

				while (!stoppingToken.IsCancellationRequested)
				{
					if (_actionsQueue.IsEmpty)
						continue;

					if (_actionsQueue.TryDequeue(out var action))
					{

					}
				}

				_commandRecorder.Stop();
			}, stoppingToken);
		}

		private async void CommandRecorded(Stream audio)
		{
			IS2TConverter? converter;

			lock (_S2TLock)
			{
				converter = S2TConverter;
			}

			if (converter is null)
				return;

			var res = await converter
				.Convert(audio);

			if (res.IsFirst)
			{

			}
			else // error handling
			{
				_exceptionNotifier.Notify(res.Second);
			}
		}

		#region PropertyChanged

		private void S2TConverterProvider_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			lock (_S2TLock)
			{
				if (_S2TConverterProvider.Value is null)
					S2TConverter = null;
				else
				{
					S2TConverter =
						_S2TConverterProvider.Value.ConverterFactory.Invoke();
				}
			}
		}

		#endregion

		public override void Dispose()
		{
			base.Dispose();

			_S2TConverterProvider.PropertyChanged -= S2TConverterProvider_PropertyChanged;

			_commandRecorder.Dispose();
		}
	}
}
