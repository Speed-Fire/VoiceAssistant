using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAssistant.SpeechSynthesis.Helpers
{
	internal static class DictionaryHelper
	{
		public static async Task Fill(Dictionary<string, string> dictionary, Stream data)
		{
			using var sr = new StreamReader(data);

			while (!sr.EndOfStream)
			{
				var line = await sr.ReadLineAsync();
				if(line is null)
					continue;

				var words = line
					.Split('\t',
						StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

				if (words.Length < 2)
					continue;

				dictionary[words[0]] = words[1];
			}
		}
	}
}
