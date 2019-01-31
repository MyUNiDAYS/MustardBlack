using System;
using System.Text.RegularExpressions;

namespace MustardBlack.Assets.Css.Css
{
	public sealed class CssPreprocessor : ICssPreprocessor
	{
		static readonly Regex fileMatch = new Regex(@"(\.css)$", RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1));

		public Regex FileMatch => fileMatch;

		public AssetProcessingResult Process(string input, string mixins = null)
		{
			return new AssetProcessingResult(AssetProcessingResult.CompilationStatus.Success, mixins + input);
		}
	}
}
