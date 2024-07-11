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
		public Task<bool> Initialize()
		{
			throw new NotImplementedException();
		}

		public Task<OneOf<bool?, Exception>> Resolve(string text)
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}
