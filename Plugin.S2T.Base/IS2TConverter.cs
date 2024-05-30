using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Core.Misc;

namespace Plugin.S2T.Base
{
	/// <summary>
	/// Speech to text converter interface.
	/// </summary>
	public interface IS2TConverter : IDisposable
	{
		/// <summary>
		/// Converts speech to text.
		/// </summary>
		/// <param name="input">Stream with an audio.</param>
		/// <param name="timeot">Timeout.</param>
		/// <returns>Recognized text or error.</returns>
		Task<OneOf<string, Exception>> Convert(Stream input, int timeot = 3000);
	}
}
