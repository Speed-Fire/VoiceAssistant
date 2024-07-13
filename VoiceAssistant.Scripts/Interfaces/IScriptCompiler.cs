using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAssistant.Scripts.Interfaces
{
	public interface IScriptCompiler
	{
		Task<Stream> Compile(string input);
	}
}
