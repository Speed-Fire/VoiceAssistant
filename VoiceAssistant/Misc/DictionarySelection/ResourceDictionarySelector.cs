using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VoiceAssistant.Misc.DictionarySelection
{
    public class ResourceDictionarySelector(string[] embeddedDictionaries)
    {
        private readonly HashSet<string> _dictionaries = [.. embeddedDictionaries];

        public IEnumerable<string> AvailableKeys => _dictionaries
					.Select(path =>
					{
						var lastSlashPos = path.LastIndexOfAny(['\\', '/']);
						var lastDotPos = path.LastIndexOf('.');

						var word = path.Substring(lastSlashPos + 1, lastDotPos - lastSlashPos - 1);

						return word;
					});

		private ResourceDictionary? _activeDictionary;
        private string? _selectedKey;

        public bool ContainsKey(string key)
        {
            return !string.IsNullOrEmpty(GetResourcePath(key));
        }

        public void Select(string key)
        {
            if (key == _selectedKey)
                return;

            var respath = GetResourcePath(key)
                ?? throw new ArgumentException($"\"{key}\" is not found");

            var ndict = new ResourceDictionary
            {
                Source = new Uri(respath)
            };

            if (_activeDictionary is not null)
                Application.Current.Resources.MergedDictionaries
                    .Remove(_activeDictionary);

            _activeDictionary = ndict;
            Application.Current.Resources
                .MergedDictionaries.Add(_activeDictionary);

            _selectedKey = key;
        }

        public void AddSourcePath(string path)
        {
            _dictionaries.Add(path);
        }

        private string? GetResourcePath(string key)
        {
            foreach (var path in _dictionaries)
            {
                var lastSlashPos = path.LastIndexOfAny(['\\', '/']);
                var lastDotPos = path.LastIndexOf('.');

                var word = path.Substring(lastSlashPos + 1, lastDotPos - lastSlashPos - 1);

                if (key == word)
                    return path;
            }

            return null;
        }
    }
}
