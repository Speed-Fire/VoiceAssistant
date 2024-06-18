using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Core.Models;

namespace VoiceAssistant.Core.Interfaces
{
	public interface IDefaultSettingsInitializer
	{
		Task Initialize(IEnumerable<Settings> defaultConfig);
	}
}
