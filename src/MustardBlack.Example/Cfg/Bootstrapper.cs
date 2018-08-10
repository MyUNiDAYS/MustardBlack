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
				var types = this.GetType().Assembly.GetTypes().Where(t => typeof(IHandler).IsAssignableFrom(t)).ToArray();
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