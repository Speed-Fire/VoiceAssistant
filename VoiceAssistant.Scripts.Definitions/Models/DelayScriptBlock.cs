using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Scripts.Models;

namespace VoiceAssistant.Scripts.Definitions.Models
{
	internal class DelayScriptBlock()
		: ScriptBlock(
			"Delay",
			[Tuple.Create("Duration", typeof(TimeSpan))],
			true,
			[],
			["System.Threading.Tasks"],
			[],
			"await Task.Delay((int)Duration.TotalMilliseconds);"
			)
	{
	}
}
