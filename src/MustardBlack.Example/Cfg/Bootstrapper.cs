using System;
using System.Collections.Generic;
using System.Linq;
using MustardBlack.Handlers;
using MustardBlack.Handlers.Binding;
using NanoIoC;

namespace MustardBlack.Example.Cfg
{
	sealed class Bootstrapper : IBootstrapper
	{
		public void Bootstrap()
		{
			Container.Global.RunAllTypeProcessors();
			Container.Global.RunAllRegistries();

			Container.Global.RemoveAllRegistrationsAndInstancesOf<IHandlerCache>();
			Container.Global.Register<IHandlerCache>(c =>
			{
				var cache = new HandlerCache(Container.Global);

				IEnumerable<Type> types = this.GetType().Assembly.GetTypes();
				types = types.Union(typeof(MustardBlack.Url).Assembly.GetTypes());
				types = types.Where(t => typeof(IHandler).IsAssignableFrom(t)).ToArray();

				cache.Warm(types);
				return cache;
			});

			BinderCollection.Initialize(Container.Global);
		}

		public void Dispose()
		{
			
		}
	}
}