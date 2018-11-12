using System.Buffers;
using System.Text.Encodings.Web;
using MuonLab.Validation;
using MustardBlack.Applications;
using MustardBlack.Example.Areas.Example;
using MustardBlack.Hosting;
using MustardBlack.Hosting.AspNet;
using MustardBlack.ViewEngines;
using MustardBlack.ViewEngines.Razor;
using MustardBlack.ViewEngines.Razor.Internal;
using NanoIoC;

namespace MustardBlack.Example.Cfg
{
	sealed class Registry : IContainerRegistry
	{
		public void Register(IContainer container)
		{
			container.Register<IApplication, ExampleApplication>();
			container.Register<IResolverContainer>(c => c);

			container.Register<IFileSystem, AspNetFileSystem>();

			container.Register<IErrorMessageResolver, DefaultErrorMessageResolver>();

			container.Register<IViewLocator, DebugRazorViewLocator>();

			container.Inject<HtmlEncoder>(HtmlEncoder.Default);

			container.Inject(ArrayPool<ViewBufferValue>.Shared);
			container.Inject(ArrayPool<char>.Shared);
			container.Register<IViewRenderer, RazorViewRenderer>();

			container.Register<IViewBufferScope, MemoryPoolViewBufferScope>(Lifecycle.HttpContextOrExecutionContextLocal);
		}
	}
}
