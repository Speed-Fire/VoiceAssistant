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
		private readonly CommandResolver _commandResolver;
		private readonly Provider<IS2TConverter> _S2TConverterProvider;
		private readonly IExceptionNotifier _exceptionNotifier;
		private readonly IVoiceAssistantMonitor _voiceAssistantMonitor;

		private readonly ConcurrentQueue<AssistantAction> _actionsQueue;

		public VoiceAssistantService(
			CommandRecorder commandRecorder,
			Provider<IS2TConverter> s2TConverterProvider,
			IExceptionNotifier exceptionNotifier,
			CommandResolver commandResolver,
			IVoiceAssistantMonitor voiceAssistantMonitor)
		{
			_commandRecorder = commandRecorder;
			_commandRecorder.CommandRecorded += CommandRecorded;

			_actionsQueue = new();
			_S2TConverterProvider = s2TConverterProvider;

			_exceptionNotifier = exceptionNotifier;
			_commandResolver = commandResolver;

			_voiceAssistantMonitor = voiceAssistantMonitor;
			_voiceAssistantMonitor.PropertyChanged += VoiceAssistantMonitor_PropertyChanged;
		}

		protected override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			return Task.Run(async () =>
			{
				if (!await _commandResolver.Initialize())
				{
					_voiceAssistantMonitor.Block();
				}
				else
				{
					_commandRecorder.Start();
				}

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
			IS2TConverter? converter = _S2TConverterProvider.Value;

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

			_voiceAssistantMonitor.PropertyChanged -= VoiceAssistantMonitor_PropertyChanged;
		}
	}
}
