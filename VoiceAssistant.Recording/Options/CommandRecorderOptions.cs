using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAssistant.Recording.Options
{
	public class CommandRecorderOptions
	{
		public int MaxSilenceDuration { get; set; }
	}
}
