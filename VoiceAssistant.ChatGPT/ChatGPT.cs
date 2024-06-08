using Python.Runtime;
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

		private readonly PyObject _instance;

		internal ChatGPT(PyObject instance)
		{
			_instance = instance;
		}

		public string SystemCommand
		{
			get
			{
				using (Py.GIL())
				{
					return _instance.GetAttr(MEMBER_SYSTEM_COMMAND).As<string>();
				}
			}
			set
			{
				using (Py.GIL())
				{
					_instance.SetAttr(MEMBER_SYSTEM_COMMAND, new PyString(value));
				}
			}
		}

		public string Model
		{
			get
			{
				using (Py.GIL())
				{
					return _instance.GetAttr(MEMBER_MODEL).As<string>();
				}
			}
			set
			{
				using (Py.GIL())
				{
					_instance.SetAttr(MEMBER_MODEL, new PyString(value));
				}
			}
		}

		public string SendMessage(string message)
		{
			using (Py.GIL())
			{
				var res = _instance.InvokeMethod(METHOD_SEND_MESSAGE, new PyString(message));

				var str = res.As<string>();

				return str;
			}
		}
	}
}
