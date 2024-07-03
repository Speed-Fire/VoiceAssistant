using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Core.Models;

namespace Plugin.Base
{
	public interface IDIPluginRegistrator
	{
		public void RegisterPlugin(IServiceCollection services, IConfiguration config);
	}
}
