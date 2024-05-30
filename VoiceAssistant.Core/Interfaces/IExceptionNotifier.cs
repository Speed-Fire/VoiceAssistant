using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAssistant.Core.Interfaces
{
	public interface IExceptionNotifier
	{
		void Notify(Exception exception);
	}
}
