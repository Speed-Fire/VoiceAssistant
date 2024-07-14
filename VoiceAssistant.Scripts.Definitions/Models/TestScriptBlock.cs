using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Scripts.Models;

namespace VoiceAssistant.Scripts.Definitions.Models
{
	internal class TestScriptBlock()
		: ScriptBlock(
			"Test",
			[Tuple.Create("Number", typeof(int)),
			 Tuple.Create("Boolean", typeof(bool)),
			 Tuple.Create("str", typeof(string)),
			 Tuple.Create("dubble", typeof(double))],
			false,
			false,
			[],
			[],
			[],
			string.Empty)
	{
	}
}
