using LiteDB;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Core;

namespace Plugin.S2T.VK.Misc
{
	internal class LiteDbContext([FromKeyedServices(Consts.APLICATION_DATA_DBS_PATH)] string dbs_path) 
		: LiteDatabase(Path.Combine(dbs_path, "S2T.VK.db"))
	{
	}
}
