using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Synthesis;
using VoiceAssistant.SpeechSynthesis.Options;
using System.Globalization;
using System.Reflection;
using VoiceAssistant.SpeechSynthesis.Helpers;

namespace VoiceAssistant.SpeechSynthesis
{
	internal class PhraseDictionary
	{
		private readonly Dictionary<string, string> _phraseDictionary = [];

		public string this[string key] => _phraseDictionary[key];

        public CultureInfo? Culture { get; private set; }

		public async Task<bool> SetCulture(CultureInfo culture)
		{
			if (Culture is not null && Culture.Name == culture.Name)
				return true;

			var resource = GetLanguageDictionary(culture);

			if (resource is null)
				return false;

			_phraseDictionary.Clear();

			await DictionaryHelper.Fill(_phraseDictionary, resource);

			return true;
		}

		public bool IsCultureSupported(CultureInfo culture)
		{
			using var stream = GetLanguageDictionary(culture);

			return stream is not null;
		}

		private static Stream? GetLanguageDictionary(CultureInfo culture)
		{
			var name = culture.Name;

			var resourcePath = $@"VoiceAssistant.SpeechSynthesis.Languages.{name}.txt";
			var resource = Assembly.GetExecutingAssembly()
				.GetManifestResourceStream(resourcePath);

			return resource;
		}
    }
}
