using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAssistant.Services.Misc.Interfaces
{
	public enum VoiceAssistantLockStatus
	{
		Unlocked,
		Locked,
		Blocked
	}

	public interface IVoiceAssistantMonitor : INotifyPropertyChanged
	{
		bool IsListening { get; }

		/// <summary>
		/// Trying to set listening status.
		/// Can be set only if monitor's lock is unlocked.
		/// </summary>
		/// <param name="value">Listening status.</param>
		/// <returns>Current monitor's lock status.</returns>
		VoiceAssistantLockStatus TrySetStatus(bool value);

		/// <summary>
		/// Disable listening and lock the monitor's lock.
		/// </summary>
		/// <returns>True if successfully locked, false if nothing has changed.</returns>
		bool Lock();

		/// <summary>
		/// Unlock the monitor's lock and enable listening.
		/// </summary>
		/// <returns>True if successfully unlocked, false if nothing has changed.</returns>
		bool Unlock();

		/// <summary>
		/// Disable listening and block the monitor's lock.
		/// The lock can't be opened anymore, helps only application restart.
		/// </summary>
		/// <returns>True if successfully blocked, false if nothing has changed.</returns>
		internal bool Block();
	}
}
