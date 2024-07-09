using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAssistant.CommandResolving.Options
{
	public class CommandResolverOptions
	{
		public bool AutomaticResolverSelection { get; set; }
		public required string SelectedResolver { get; set; }
	}
}
