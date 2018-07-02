namespace MustardBlack.Assets.Css
{
	public interface ICssPreprocessor
	{
		AssetProcessingResult Process(string input, string mixins = null);
	}
}
