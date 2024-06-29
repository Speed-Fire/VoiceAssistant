using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAssistant.Services.Entities
{
	public abstract class PublicValidator : ObservableValidator
	{
		public void ValidateAll()
		{
			ValidateAllProperties();
		}
	}
}
