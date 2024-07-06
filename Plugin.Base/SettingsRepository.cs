using Plugin.Base.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace Plugin.Base
{
	public sealed class SettingsRepository : IDisposable
	{
        private readonly string _filePath;
		private readonly JsonObject _settingsRoot;

        private readonly Mutex _mutex;

        private bool _disposed = false;

        public SettingsRepository(string filePath)
        {
            _mutex = new(true, $"Synergy.VoiceAssistant.{filePath}".Replace("\\", "").Replace("/", ""));

            _filePath = filePath;
            using var stream = File.OpenRead(filePath);

            _settingsRoot = JsonNode
                .Parse(stream, documentOptions: new() { AllowTrailingCommas = true })!
                .AsObject();
        }

		public T? GetValue<T>(string key)
		{
			ObjectDisposedException.ThrowIf(_disposed, this);

			if (!_settingsRoot.TryGetNestedNode(key, out var node))
				throw new InvalidOperationException("Key not found!");

			if (node is null)
				return default;

			return node.GetValue<T>();
		}

        public string GetValueAsString(string key)
        {
			ObjectDisposedException.ThrowIf(_disposed, this);

			if (!_settingsRoot.TryGetNestedNode(key, out var node))
				throw new InvalidOperationException("Key not found!");

            if (node is null)
                return string.Empty;

            return node.ToString();
		}

        public void SetValue(string key, object value)
        {
			ObjectDisposedException.ThrowIf(_disposed, this);

			_settingsRoot.SetNestedValue(key, value);
		}

        public Task SaveAsync()
        {
			ObjectDisposedException.ThrowIf(_disposed, this);

			return Task.Run(() =>
            {
                using var stream = File.Open(_filePath, FileMode.Truncate, FileAccess.Write);
                using var utf8wr = new Utf8JsonWriter(stream);

				var json = _settingsRoot.ToJsonString(new()
				{
					WriteIndented = true,
					TypeInfoResolver = new DefaultJsonTypeInfoResolver()
				});

				utf8wr.WriteRawValue(json);
			});
        }

        public void Save()
        {
            SaveAsync().Wait();
        }

		public void Dispose()
		{
            if(_disposed)
                return;

			_mutex.ReleaseMutex();
            _mutex.Dispose();

            _disposed = true;
		}
	}
}
