using MustardBlack.Assets.Css;
using MustardBlack.Assets.Less;
using MustardBlack.Assets.Sass;
using MustardBlack.TempData;
using NanoIoC;

namespace MustardBlack.NanoIoC
{
	public class MustardBlackRegistry : IContainerRegistry
	{
		public void Register(IContainer container)
		{
			container.Register<ITempDataMechanism, CookieTempDataMechanism>();
			container.Register<ICssPreprocessor, LessCssPreprocessor>();
			container.Register<ICssPreprocessor, SassCssPreprocessor>();
		}
	}
}
