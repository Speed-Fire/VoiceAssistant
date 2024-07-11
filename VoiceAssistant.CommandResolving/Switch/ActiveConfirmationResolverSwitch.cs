using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.CommandResolving.Options;
using VoiceAssistant.CommandResolving.TextResolving.Resolvers;
using VoiceAssistant.CommandResolving.TextResolving.Resolvers.Confirmation;

namespace VoiceAssistant.CommandResolving.Switch
{
	internal class ActiveConfirmationResolverSwitch(
		DummyConfirmationResolver dummyResolver,
		SmartConfirmationResolver smartResolver,
		IOptions<TextResolvingOptions> options) 
		: ActiveTextResolverSwitch<bool?>(
				  dummyResolver, 
				  smartResolver,
				  options.Value.PreferredConfirmationResolver)
	{
	}
}
