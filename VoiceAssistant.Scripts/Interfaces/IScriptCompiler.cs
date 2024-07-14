using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Common;

namespace VoiceAssistant.Scripts.Interfaces
{
	public interface IScriptCompiler
	{
		Task<OneOf<MemoryStream, Exception>> Compile(string input);
	}
}
