using MustardBlack.Assets.Css;
using MustardBlack.Assets.Javascript;
using NanoIoC;

namespace MustardBlack.NanoIoC
{
	public class MustardBlackRegistry : IContainerRegistry
	{
		public void Register(IContainer container)
		{
			container.Register<IJavascriptCompressor, YuiJavascriptCompressor>();
			container.Register<ICssPreprocessor, LessCssPreprocessor>();
		}
	}
}
