using VoiceAssistant.Common;

namespace VoiceAssistant.Core.Interfaces
{
	public interface IApplicationSettingsService
	{
		/// <summary>
		/// Gets value in current section on specified key.
		/// </summary>
		/// <param name="key">Key. Must not have leading or tailing ':'.</param>
		/// <returns>Settings value.</returns>
		Task<OneOf<string, Exception>> GetValueAsync(string key);

		/// <summary>
		/// Sets value in current section on specified key.
		/// </summary>
		/// <param name="key">Key. Must not have leading or tailing ':'.</param>
		/// <param name="value">Value.</param>
		/// <returns></returns>
		Task<Exception?> SetValueAsync(string key, string value);

		/// <summary>
		/// Sets current section.
		/// Automatically appends "Application:" in the beginning.
		/// </summary>
		/// <param name="section">Section.</param>
		void SetCurrentSection(string? section);
	}
}