using MustardBlack.Assets.Css;
using MustardBlack.Assets.Css.Css;
using MustardBlack.Assets.Css.Less;
using MustardBlack.Assets.Css.Sass;
using MustardBlack.TempData;
using NanoIoC;

namespace MustardBlack.NanoIoC
{
	public class MustardBlackRegistry : IContainerRegistry
	{
		public void Register(IContainer container)
		{
			container.Register<ITempDataMechanism, CookieTempDataMechanism>();
			container.Register<ICssPreprocessor, CssPreprocessor>();
			container.Register<ICssPreprocessor, LessCssPreprocessor>();
			container.Register<ICssPreprocessor, SassCssPreprocessor>();
		}
	}
}
