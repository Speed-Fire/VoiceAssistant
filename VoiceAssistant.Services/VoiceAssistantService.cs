using Microsoft.Extensions.Hosting;
using Plugin.S2T.Base;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.CommandResolving;
using VoiceAssistant.Core.Interfaces;
using VoiceAssistant.Common;
using VoiceAssistant.Domain.Models;
using VoiceAssistant.Recording;
using VoiceAssistant.Services.Misc.Interfaces;
using VoiceAssistant.CommandResolving.TextResolving.Services;
using Microsoft.Extensions.Logging;

namespace VoiceAssistant.Services
{
	public sealed class VoiceAssistantService : BackgroundService
	{
		private readonly struct PendingAssistantAction(AssistantAction action)
		{
			public AssistantAction Action { get; } = action;
			public DateTime PendingStartTime { get; } = DateTime.Now;
		}

		private enum ConfirmationState
		{
			Pending,
			Confirmed,
			Denied
		}

		private readonly SpeechRecorder _speechRecorder;
		private readonly ITextResolvingService _textResolvingService;
		private readonly SpeechToTextService _speechToTextService;
		private readonly IAssistantVoice _assistantVoice;
		private readonly IUrgentNotifier _urgentNotifier;
		private readonly IVoiceAssistantMonitor _voiceAssistantMonitor;
		private readonly ILogger _logger;

		private readonly ConcurrentQueue<AssistantAction> _actionsQueue;
		private readonly TimeSpan ConfirmationTimeout = TimeSpan.FromSeconds(10);
		private readonly SemaphoreSlim _semaphore = new(1, 1);

		private PendingAssistantAction? _pendingAction;
		private ConfirmationState? _confirmationState;

		public VoiceAssistantService(
			SpeechRecorder speechRecorder,
			ITextResolvingService textResolvingService,
			SpeechToTextService speechToTextService,
			IAssistantVoice assistantVoice,
			IUrgentNotifier urgentNotifier,
			IVoiceAssistantMonitor voiceAssistantMonitor,
			ILogger<VoiceAssistantService> logger)
		{
			_speechRecorder = speechRecorder;
			_urgentNotifier = urgentNotifier;
			_logger = logger;

			_actionsQueue = new();
			_speechToTextService = speechToTextService;
			_speechRecorder.SpeechRecorded += _speechToTextService.Convert;
			_speechToTextService.Converted += CommandRecorded;
			_speechToTextService.ConversionFailed += NotifyError;

			_assistantVoice = assistantVoice;
			_textResolvingService = textResolvingService;

			_voiceAssistantMonitor = voiceAssistantMonitor;
			_voiceAssistantMonitor.PropertyChanged += VoiceAssistantMonitor_PropertyChanged;
		}

		public override async Task StartAsync(CancellationToken cancellationToken)
		{
			_logger.LogInformation("Initializing...");

			var initializationResult = await Initialize();

			if (!initializationResult)
			{
				_logger.LogInformation("Initialization failed. Assistant won't be recognizing speech.");
				_voiceAssistantMonitor.Block();
			}
			else
			{
				_logger.LogInformation("Initialization completed.");
				_speechRecorder.Start();
			}

			await base.StartAsync(cancellationToken);
		}

		protected override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			return Task.Run(async () =>
			{
				while (!stoppingToken.IsCancellationRequested)
				{
					if(_pendingAction is not null)
					{
						var isNotPendingAnymore = await TryExecutePendingAction();
						await TryDropPendingAction(isNotPendingAnymore);

						continue;
					}

					if (_actionsQueue.IsEmpty)
						continue;

					if (_actionsQueue.TryDequeue(out var action))
					{
						if (action.NeedsConfirmation)
						{
							await PutPendingAction(action);
						}
						else
						{
							await ExecuteAction(action);
						}
					}
				}

				_speechRecorder.Stop();
			}, stoppingToken);
		}

		private async void CommandRecorded(string text)
		{
			Exception? ex = null;

			if (_confirmationState == ConfirmationState.Pending)
			{
				await _semaphore.WaitAsync();

				if (_confirmationState == ConfirmationState.Pending)
				{
					ex = await ResolveConfirmation(text);
				}

				_semaphore.Release();
			}
			else
			{
				ex = await ResolveCommand(text);
			}

			// error handling
			if (ex is not null)
				NotifyError(ex);
		}

		private void NotifyError(Exception obj)
		{
			if(obj is VoicableException voiceEx)
			{
				_ = _assistantVoice.Speak(voiceEx);
			}
			else
			{
				_logger.LogError(obj, "Something went wrong.");
				_urgentNotifier.NotifyError(string.Empty, exception: obj);
			}
		}

		private async Task<bool> Initialize()
		{
			var textResolvingInit = _textResolvingService.Initialize();
			var assistantVoiceInit = _assistantVoice.Initialize();

			await assistantVoiceInit;
			return await textResolvingInit;
		}

		#region Pending action

		private async Task PutPendingAction(AssistantAction action)
		{
			_logger.LogInformation("Action needs confirmation. Mark action as pending...");

			await _assistantVoice.Speak("Commands.Confirmation.Request");
			_speechRecorder.SetMode(SpeechRecognitionMode.Loop);

			_pendingAction = new(action);
			_confirmationState = ConfirmationState.Pending;
		}

		private async Task TryDropPendingAction(bool isNotPendingAnymore)
		{
			if (isNotPendingAnymore ||
				DateTime.Now - _pendingAction!.Value.PendingStartTime >= ConfirmationTimeout)
			{
				await _semaphore.WaitAsync();

				_pendingAction = null;
				_confirmationState = null;

				_speechRecorder.SetMode(SpeechRecognitionMode.OnKeyword);

				_logger.LogInformation("Pending action is dropped.");

				_semaphore.Release();
			}
		}

		/// <summary>
		/// Tries to execute pending assistant action.
		/// If action still pends, then return false.
		/// If action is confirmed or denied, return true;
		/// </summary>
		/// <returns></returns>
		private async Task<bool> TryExecutePendingAction()
		{
			if (_confirmationState!.Value != ConfirmationState.Pending)
			{
				await _semaphore.WaitAsync();

				if (_confirmationState!.Value != ConfirmationState.Pending)
				{
					await _assistantVoice.Speak("Принято");

					if (_confirmationState.Value == ConfirmationState.Confirmed)
					{
						await ExecuteAction(_pendingAction!.Value.Action);
					}

					_pendingAction = null;
					_confirmationState = null;

					return true;
				}

				_semaphore.Release();
			}

			return false;
		}

		#endregion

		#region Command resolving

		private async Task<Exception?> ResolveCommand(string command)
		{
			var resolvingResult =
					await _textResolvingService.ResolveCommand(command);

			if (resolvingResult.IsFirst)
			{
				_actionsQueue.Enqueue(resolvingResult.First);
			}
			else
			{
				return resolvingResult.Second;
			}

			return null;
		}

		private async Task<Exception?> ResolveConfirmation(string text)
		{
			var resolvingResult =
					await _textResolvingService.ResolveConfirmation(text);

			if (resolvingResult.IsFirst)
			{
				var confirmationResult = resolvingResult.First;

				if(confirmationResult == true)
				{
					_confirmationState = ConfirmationState.Confirmed;
				}
				else if(confirmationResult == false)
				{
					_confirmationState = ConfirmationState.Denied;
				}
			}
			else
			{
				return resolvingResult.Second;
			}

			return null;
		}

		#endregion

		#region Assistant action execution

		private Task ExecuteAction(AssistantAction action)
		{
			_logger.LogInformation("Executing action...");

			return Task.CompletedTask;
		}

		#endregion

		#region PropertyChanged

		private void VoiceAssistantMonitor_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if(e.PropertyName == nameof(IVoiceAssistantMonitor.IsListening))
			{
				if(_voiceAssistantMonitor.IsListening)
				{
					_speechRecorder.Start();
				}
				else
				{
					_speechRecorder.Stop();
				}
			}
		}

		#endregion

		#region Dispose

		public override void Dispose()
		{
			base.Dispose();

			_speechRecorder.SpeechRecorded -= _speechToTextService.Convert;
			_speechToTextService.Converted -= CommandRecorded;
			_speechToTextService.ConversionFailed -= NotifyError;

			_voiceAssistantMonitor.PropertyChanged -= VoiceAssistantMonitor_PropertyChanged;
		}

		#endregion
	}
}
