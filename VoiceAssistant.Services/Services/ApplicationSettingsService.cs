using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Core.Models;
using VoiceAssistant.DAL.Repositories;

namespace VoiceAssistant.Services
{
	public class ApplicationSettingsService(IAsyncRepository<Settings> settings)
	{
		private const string DEFAULT_SECTION = "Application";

		private readonly IAsyncRepository<Settings> _settings = settings;

		private string CurrentSection { get; set; } = $"{DEFAULT_SECTION}:";

		/// <summary>
		/// Gets value in current section on specified key.
		/// </summary>
		/// <param name="key">Key. Must not have leading or tailing ':'.</param>
		/// <returns>Settings value.</returns>
		public async Task<string> GetValueAsync(string key)
		{
			var fullKey = CurrentSection + key;

			var setting = await _settings.GetAll().FirstAsync(s => s.Id == fullKey);

			return setting.Value;
		}

		/// <summary>
		/// Sets value in current section on specified key.
		/// </summary>
		/// <param name="key">Key. Must not have leading or tailing ':'.</param>
		/// <param name="value">Value.</param>
		/// <returns></returns>
		public async Task SetValueAsync(string key, string value)
		{
			var fullKey = CurrentSection + key;

			var setting = await _settings.GetAll().FirstAsync(s => s.Id == fullKey);

			setting.Value = value;

			await _settings.Update(setting);
		}

		/// <summary>
		/// Sets current section.
		/// Automatically appends "Application:" in the beginning.
		/// </summary>
		/// <param name="section">Section.</param>
		public void SetCurrentSection(string? section)
		{
			var sb = new StringBuilder();
			sb.Append(DEFAULT_SECTION);

			if (!string.IsNullOrWhiteSpace(section))
			{
				if (!section.StartsWith(':'))
					sb.Append(':');

				sb.Append(section);
			}

			if (sb[^1] != ':')
				sb.Append(':');

			CurrentSection = sb.ToString();
		}

		private string GetFullKey(string key)
		{
			return CurrentSection + key;
		}
	}
}
