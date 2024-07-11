using MML_ConfirmationClassifier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Common;

namespace VoiceAssistant.CommandResolving.TextResolving.Resolvers.Confirmation
{
	internal class DummyConfirmationResolver : ITextResolver<bool?>
	{
		public bool IsInitialized => true;

		public Task<bool> Initialize()
		{
			return Task.FromResult(true);
		}

		public Task<OneOf<bool?, Exception>> Resolve(string text)
		{
			return Task.Run<OneOf<bool?, Exception>>(() =>
			{
				//Load sample data
				var sampleData = new ConfirmationClassifierModel.ModelInput()
				{
					Sentence = text.ToLower(),
				};

				//Load model and predict output
				var prediction = ConfirmationClassifierModel.Predict(sampleData);

				if (prediction.ConfirmationResult == 0)
					return new(false);
				else if(prediction.ConfirmationResult == 1)
					return new(true);
				else
					return new((bool?)null);
			});
		}

		public void Dispose() { }
	}
}
