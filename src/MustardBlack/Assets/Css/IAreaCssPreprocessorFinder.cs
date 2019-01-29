namespace MustardBlack.Assets.Css
{
	public interface IAreaCssPreprocessorFinder
	{
		ICssPreprocessor FindCssPreprocessorForArea(string areaName);
	}
}