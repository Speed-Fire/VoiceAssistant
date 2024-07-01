using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Services.Misc.Interfaces;

namespace VoiceAssistant.Services.Misc.Implementations
{
	internal class VoiceAssistantMonitor :
		IVoiceAssistantMonitor
	{
		private readonly object _threadLocker = new();
		public event PropertyChangedEventHandler? PropertyChanged;

		private volatile VoiceAssistantLockStatus _status = VoiceAssistantLockStatus.Unlocked;

		private volatile bool _isListening = true;
		public bool IsListening
		{
			get => _isListening;

			private set
			{
				if(_isListening == value) 
					return;

				_isListening = value;

				NotifyChanged();
			}
		}

		public VoiceAssistantLockStatus TrySetStatus(bool value)
		{
			if (_status == VoiceAssistantLockStatus.Unlocked)
			{
				lock (_threadLocker)
				{
					if (_status == VoiceAssistantLockStatus.Unlocked)
					{
						IsListening = value;

						return _status;
					}
					else
						return _status;
				}
			}
			else
				return _status;
		}

		public bool Lock()
		{
			if (_status == VoiceAssistantLockStatus.Unlocked)
			{
				lock (_threadLocker)
				{
					if (_status == VoiceAssistantLockStatus.Unlocked)
					{
						_status = VoiceAssistantLockStatus.Locked;

						IsListening = false;

						return true;
					}
					else
						return false;
				}
			}
			else
				return false;
		}

		public bool Unlock()
		{
			if (_status == VoiceAssistantLockStatus.Locked)
			{
				lock (_threadLocker)
				{
					if (_status == VoiceAssistantLockStatus.Locked)
					{
						_status = VoiceAssistantLockStatus.Unlocked;

						IsListening = true;

						return true;
					}
					else
						return false;
				}
			}
			else
				return false;
		}

		bool IVoiceAssistantMonitor.Block()
		{
			if(_status != VoiceAssistantLockStatus.Blocked)
			{
				lock(_threadLocker)
				{
					if (_status != VoiceAssistantLockStatus.Blocked)
					{
						_status = VoiceAssistantLockStatus.Blocked;

						IsListening = false;

						return true;
					}
					else
						return false;
				}
			}
			else
				return false;
		}

		private void NotifyChanged()
		{
			PropertyChanged?.Invoke(this, new(nameof(IsListening)));
		}
	}
}
