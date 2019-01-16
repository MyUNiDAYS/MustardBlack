using System.Text;

namespace MustardBlack.Assets.Css
{
	public sealed class SassCssPreprocessor : ICssPreprocessor
	{
		const string sassCompilerSeparatorColorRed = ".sass-compiler-separator{color:red}";

		public AssetProcessingResult Process(string input, string mixins = null)
		{
			var lessBuilder = new StringBuilder();

			if (!string.IsNullOrEmpty(mixins))
			{
				// This exists because our LessCompiler this was ported from doesnt let us omit the mixins after input compilation.
				// Maybe we can be cleaner with the Sass here
				lessBuilder.Append(sassCompilerSeparatorColorRed);
			}

			lessBuilder.Append(input);

			var cssCompilationResult = SassCompiler.TryCompile(lessBuilder.ToString(), mixins);

			if (cssCompilationResult.Status != AssetProcessingResult.CompilationStatus.Success || string.IsNullOrEmpty(mixins))
				return cssCompilationResult;

			var css = cssCompilationResult.Result.Substring(cssCompilationResult.Result.IndexOf(sassCompilerSeparatorColorRed) + sassCompilerSeparatorColorRed.Length);
			return new AssetProcessingResult(AssetProcessingResult.CompilationStatus.Success, css);
		}
	}
}
