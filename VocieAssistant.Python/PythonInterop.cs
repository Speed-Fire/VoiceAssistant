using Python.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VocieAssistant.Python
{
	public class PythonInterop
	{
		public void Initialize()
		{
			string pythonDll = @"C:\Users\Р’Р»Р°Рґ\AppData\Local\Programs\Python\Python38\python38.dll";
			Runtime.PythonDLL = pythonDll;
			PythonEngine.Initialize();
		}

		public void AddPath(string path)
		{
			using (Py.GIL())
			{
				dynamic sys = Py.Import("sys");
				sys.path.append(Path.Combine(path));
			}
		}

		public void RunPythonCode(string pycode)
		{
			using (Py.GIL())
			{
				PythonEngine.RunSimpleString(pycode);
			}
		}
	}
}
