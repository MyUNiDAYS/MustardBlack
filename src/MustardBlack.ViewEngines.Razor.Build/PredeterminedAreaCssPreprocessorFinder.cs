using MustardBlack.Assets.Css;
using MustardBlack.Assets.Less;
using MustardBlack.Assets.Sass;

namespace MustardBlack.ViewEngines.Razor.Build
{
	sealed class PredeterminedAreaCssPreprocessorFinder : IAreaCssPreprocessorFinder
	{
		readonly string mode;

		public PredeterminedAreaCssPreprocessorFinder(string mode)
		{
			this.mode = mode;
		}

		public ICssPreprocessor FindCssPreprocessorForArea(string areaName)
		{
			return this.mode == "sass" ? new SassCssPreprocessor() as ICssPreprocessor : new LessCssPreprocessor();
		}
	}
}