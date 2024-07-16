using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Domain.Underlying;
using VoiceAssistant.Scripts.Models;

namespace VoiceAssistant.Scripts.Interfaces
{
	public interface IUnderlyingScriptService
	{
		/// <summary>
		/// Determines whether script can be updated without disabling binded Actions to it.
		/// </summary>
		/// <param name="script"></param>
		/// <returns></returns>
		Task<bool> CanScriptBeFreeUpdated(UnderlyingScript script);
		Task<Exception?> UpdateScript(UnderlyingScript script);
	}
}
