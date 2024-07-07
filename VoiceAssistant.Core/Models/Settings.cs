using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAssistant.Core.Models
{
	public class Settings(string id, string value, bool applicationRestartNeeded = false)
	{
		public string Id { get; private set; } = id;
		public string Value { get; set; } = value;
		public bool ApplicationRestartNeeded { get; private set; } = applicationRestartNeeded;
	}
}
