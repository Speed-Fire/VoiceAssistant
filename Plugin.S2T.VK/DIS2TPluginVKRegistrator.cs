using Microsoft.Extensions.DependencyInjection;
using Plugin.Base;
using Plugin.S2T.Base;
using Plugin.S2T.VK.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Core.Interfaces;

namespace Plugin.S2T.VK
{
    internal class DIS2TPluginVKRegistrator : IDIPluginRegistrator
    {
        public void RegisterPlugin(IServiceCollection services)
        {
            services
                .AddSingleton<Func<S2TConverterVK>>(provider =>
                {
                    return () =>
                    {
                        return provider.GetRequiredService<S2TConverterVK>();
                    };
                })
                .AddSingleton<S2TConverterInfo, S2TConverterInfoVK>()
                .AddTransient<ISettingsLoader, SettingsLoader>()
                .AddTransient<LiteDbContext>()
                .AddTransient<S2TConverterVK>();
        }
    }
}
