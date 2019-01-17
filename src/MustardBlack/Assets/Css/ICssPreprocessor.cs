using System.Text.RegularExpressions;

namespace MustardBlack.Assets.Css
{
	public interface ICssPreprocessor
	{
		/// <summary>
		/// Regex matched against all asset files to determine whether to include them for preprocessing or not
		/// </summary>
		Regex FileMatch { get; }

		AssetProcessingResult Process(string input, string mixins = null);
	}
}
