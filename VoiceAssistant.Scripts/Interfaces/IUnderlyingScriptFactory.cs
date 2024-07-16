using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Common;
using VoiceAssistant.Domain.Underlying;
using VoiceAssistant.Scripts.Models;

namespace VoiceAssistant.Scripts.Interfaces
{
	public interface IUnderlyingScriptFactory
	{
		OneOf<UnderlyingScript, Exception> Create(Stream stream, long id, bool leaveOpen = false);
	}
}
