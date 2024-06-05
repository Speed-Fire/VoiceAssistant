using CSPythonInvoker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAssistant.ChatGPT
{
	public class ChatGPT
	{
		private const string MEMBER_SYSTEM_COMMAND = "systemCommand";
		private const string MEMBER_MODEL = "model";
		private const string METHOD_SEND_MESSAGE = "sendMessage";

		private readonly PInstance _instance;

		internal ChatGPT(PInstance instance)
		{
			_instance = instance;
		}

		public string SystemCommand
		{
			get => _instance.GetMember<string>(MEMBER_SYSTEM_COMMAND);
			set => _instance.SetMember(MEMBER_SYSTEM_COMMAND, value);
		}

		public string Model
		{
			get => _instance.GetMember<string>(MEMBER_MODEL);
			set => _instance.SetMember(MEMBER_MODEL, value);
		}

		public string SendMessage(string message)
		{
			return _instance.CallFunction(METHOD_SEND_MESSAGE, message);
		}
	}
}
