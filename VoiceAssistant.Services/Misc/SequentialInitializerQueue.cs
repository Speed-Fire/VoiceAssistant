using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Core.Interfaces;

namespace VoiceAssistant.Services.Misc
{
	public class SequentialInitializerQueue(IServiceProvider services)
	{
		private readonly IServiceProvider _services = services;
		private readonly Queue<Type> _queue = [];

		public SequentialInitializerQueue Add<TInitializer>()
			where TInitializer : ISequentialInitializer
		{
			_queue.Enqueue(typeof(TInitializer));

			return this;
		}

		public Task Execute()
		{
			return Task.Run(async () =>
			{
				while(_queue.Count > 0)
				{
					var type = _queue.Dequeue();
					var initializer = (ISequentialInitializer)_services.GetRequiredService(type);

					await initializer.Initialize();
				}
			});
		}
	}
}
