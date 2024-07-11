using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.CommandResolving.Options;
using VoiceAssistant.CommandResolving.TextResolving.Resolvers;
using VoiceAssistant.CommandResolving.TextResolving.Resolvers.Commands;
using VoiceAssistant.Domain.Models;

namespace VoiceAssistant.CommandResolving.Switch
{
	internal class ActiveCommandResolverSwitch(
		DummyCommandResolver dummyResolver,
		SmartCommandResolver smartResolver,
		IOptions<TextResolvingOptions> options)
		: ActiveTextResolverSwitch<AssistantAction>(
			dummyResolver, 
			smartResolver, 
			options.Value.PreferredCommandResolver)
	{
	}
}
