using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Core.Misc;

namespace VoiceAssistant.Services.Misc.Interfaces
{
	public interface IProviderInitializer
	{
		Task<bool> InitializeAsync();
	}
}
