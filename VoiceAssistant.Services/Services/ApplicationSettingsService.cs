using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Common;
using VoiceAssistant.Core.Models;
using VoiceAssistant.DAL.Repositories;

namespace VoiceAssistant.Services
{
	public class ApplicationSettingsService(IAsyncRepository<Settings> settings)
	{
		private const string DEFAULT_SECTION = "Application";

		private readonly IAsyncRepository<Settings> _settings = settings;
		private readonly SemaphoreSlim _semaphore = new(1, 1);

		private string CurrentSection { get; set; } = $"{DEFAULT_SECTION}:";

		/// <summary>
		/// Gets value in current section on specified key.
		/// </summary>
		/// <param name="key">Key. Must not have leading or tailing ':'.</param>
		/// <returns>Settings value.</returns>
		public async Task<OneOf<string, Exception>> GetValueAsync(string key)
		{
			try
			{
				await _semaphore.WaitAsync();

				var fullKey = CurrentSection + key;

				var setting = await _settings.GetAll().FirstAsync(s => s.Id == fullKey);

				return new(setting.Value);
			}
			catch (Exception ex)
			{
				return new(ex);
			}
			finally
			{
				_semaphore.Release();
			}
		}

		/// <summary>
		/// Sets value in current section on specified key.
		/// </summary>
		/// <param name="key">Key. Must not have leading or tailing ':'.</param>
		/// <param name="value">Value.</param>
		/// <returns></returns>
		public async Task<Exception?> SetValueAsync(string key, string value)
		{
			try
			{
				await _semaphore.WaitAsync();

				var fullKey = CurrentSection + key;

				var setting = await _settings.GetAll().FirstAsync(s => s.Id == fullKey);

				setting.Value = value;

				await _settings.Update(setting);

				return null;
			}
			catch (Exception ex)
			{
				return ex;
			}
			finally 
			{
				_semaphore.Release(); 
			}
		}

		/// <summary>
		/// Sets current section.
		/// Automatically appends "Application:" in the beginning.
		/// </summary>
		/// <param name="section">Section.</param>
		public void SetCurrentSection(string? section)
		{
			_semaphore.Wait();

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

			_semaphore.Release();
		}
	}
}
