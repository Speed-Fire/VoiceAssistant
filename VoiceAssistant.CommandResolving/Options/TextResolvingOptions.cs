using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAssistant.CommandResolving.Options
{
	public class TextResolvingOptions
	{
		public string PreferredCommandResolver { get; set; } = string.Empty; 
		public string PreferredConfirmationResolver {  get; set; } = string.Empty; 
	}
}
