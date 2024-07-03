using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAssistant.Common
{
	public class VoicableException(CultureInfo culture, string message) : Exception(message)
	{
		public CultureInfo Culture { get; } = culture;
	}
}
