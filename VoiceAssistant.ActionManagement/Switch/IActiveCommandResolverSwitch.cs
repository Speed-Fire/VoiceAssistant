using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAssistant.ActionManagement.Switch
{
	public interface IActiveCommandResolverSwitch
	{
		IEnumerable<string> AvailableResolvers { get; }

		internal Task<bool> Initialize();
		internal ICommandResolver GetActiveResolver();
		internal void SignalCurrentResolverError();
	}
}
