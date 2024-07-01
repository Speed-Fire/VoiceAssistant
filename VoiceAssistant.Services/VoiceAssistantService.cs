using Microsoft.Extensions.Hosting;
using Plugin.S2T.Base;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.ActionManagement;
using VoiceAssistant.Core.Interfaces;
using VoiceAssistant.Core.Misc;
using VoiceAssistant.Domain.Models;
using VoiceAssistant.Recording;
using VoiceAssistant.Services.Misc.Interfaces;

namespace VoiceAssistant.Services
{
	public sealed class VoiceAssistantService : BackgroundService
	{
		private readonly CommandRecorder _commandRecorder;
		private readonly ICommandResolver _commandResolver;
		private readonly Provider<S2TConverterInfo> _S2TConverterProvider;
		private readonly IExceptionNotifier _exceptionNotifier;
		private readonly IVoiceAssistantMonitor _voiceAssistantMonitor;

		private readonly ConcurrentQueue<AssistantAction> _actionsQueue;
		private readonly object _S2TLock = new();

		private IS2TConverter? S2TConverter { get; set; }

		public VoiceAssistantService(
			CommandRecorder commandRecorder,
			Provider<S2TConverterInfo> s2TConverterProvider,
			IExceptionNotifier exceptionNotifier,
			ICommandResolver commandResolver,
			IVoiceAssistantMonitor voiceAssistantMonitor)
		{
			_commandRecorder = commandRecorder;
			_commandRecorder.CommandRecorded += CommandRecorded;

			_actionsQueue = new();
			_S2TConverterProvider = s2TConverterProvider;
			_S2TConverterProvider.PropertyChanged += S2TConverterProvider_PropertyChanged;

			_exceptionNotifier = exceptionNotifier;
			_commandResolver = commandResolver;

			_voiceAssistantMonitor = voiceAssistantMonitor;
			_voiceAssistantMonitor.PropertyChanged += VoiceAssistantMonitor_PropertyChanged;
		}

		protected override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			return Task.Run(async () =>
			{
				await _commandResolver.Initialize();

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
			Exception? ex = null;
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
				var resolvingResult =
					await _commandResolver.Resolve(res.First);

				if (resolvingResult.IsFirst)
				{
					_actionsQueue.Enqueue(resolvingResult.First);
				}
				else
				{
					ex = resolvingResult.Second;
				}
			}
			else
				ex = res.Second;

			// error handling
			if (ex is not null)
				_exceptionNotifier.Notify(res.Second);
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

		private void VoiceAssistantMonitor_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if(e.PropertyName == nameof(IVoiceAssistantMonitor.IsListening))
			{
				if(_voiceAssistantMonitor.IsListening)
				{
					_commandRecorder.Start();
				}
				else
				{
					_commandRecorder.Stop();
				}
			}
		}

		#endregion

		public override void Dispose()
		{
			base.Dispose();

			_S2TConverterProvider.PropertyChanged -= S2TConverterProvider_PropertyChanged;
			_voiceAssistantMonitor.PropertyChanged -= VoiceAssistantMonitor_PropertyChanged;

			_commandRecorder.Dispose();
		}
	}
}
