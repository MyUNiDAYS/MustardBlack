using MustardBlack.Assets.Css;
using MustardBlack.Assets.Javascript;
using MustardBlack.TempData;
using NanoIoC;

namespace MustardBlack.NanoIoC
{
	public class MustardBlackRegistry : IContainerRegistry
	{
		public void Register(IContainer container)
		{
			container.Register<IJavascriptCompressor, YuiJavascriptCompressor>();
			container.Register<ICssPreprocessor, SassCssPreprocessor>();
			container.Register<ITempDataMechanism, CookieTempDataMechanism>();
		}
	}
}
