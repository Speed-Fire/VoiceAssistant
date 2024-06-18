using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAssistant.Core.Models
{
	public record class Settings(string Id, string Value, bool ApplicationRestartNeeded = false);
}
