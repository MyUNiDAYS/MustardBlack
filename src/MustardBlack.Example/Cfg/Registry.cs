using MuonLab.Validation;
using MustardBlack.Applications;
using MustardBlack.Example.Areas.Example;
using NanoIoC;

namespace MustardBlack.Example.Cfg
{
	sealed class Registry : IContainerRegistry
	{
		public void Register(IContainer container)
		{
			container.Register<IApplication, ExampleApplication>();
			container.Register<IResolverContainer>(c => c);

			container.Register<IErrorMessageResolver, DefaultErrorMessageResolver>();
		}
	}
}
