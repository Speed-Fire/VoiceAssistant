using Plugin.S2T.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Common;

namespace VoiceAssistant.Services
{
	public class SpeechToTextService(Provider<IS2TConverter> converterProvider)
	{
		private readonly Provider<IS2TConverter> _converterProvider = converterProvider;

		public event Action<string>? Converted;
		public event Action<Exception>? ConversionFailed;

		public async void Convert(Stream audio)
		{
			IS2TConverter? converter = _converterProvider.Value;

			if (converter is null)
				return;

			var res = await converter
				.Convert(audio);

			if (res.IsFirst)
			{
				Converted?.Invoke(res.First);
			}
			else
			{
				ConversionFailed?.Invoke(res.Second);
			}
		}
	}
}
