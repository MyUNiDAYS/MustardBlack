namespace MustardBlack.Assets.Css
{
	public sealed class ContainerAreaCssPreprocessorFinder : IAreaCssPreprocessorFinder
	{
		readonly ICssPreprocessor cssPreprocessor;

		public ContainerAreaCssPreprocessorFinder(ICssPreprocessor cssPreprocessor)
		{
			this.cssPreprocessor = cssPreprocessor;
		}

		public ICssPreprocessor FindCssPreprocessorForArea(string areaName)
		{
			return this.cssPreprocessor;
		}
	}
}