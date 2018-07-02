using System.Text;

namespace MustardBlack.Assets.Css
{
	public sealed class LessCssPreprocessor : ICssPreprocessor
	{
		const string lessCompilerSeparatorColorRed = ".less-compiler-separator{color:red}";

		public AssetProcessingResult Process(string input, string mixins = null)
		{
			var lessBuilder = new StringBuilder();

			if (!string.IsNullOrEmpty(mixins))
			{
				// This exists because our LessCompiler doesnt let us omit the mixins after input compilation, if it did, we could remove this
				lessBuilder.Append(lessCompilerSeparatorColorRed);
			}

			lessBuilder.Append(input);

			var cssCompilationResult = LessCompiler.TryCompile(lessBuilder.ToString(), mixins);

			if (cssCompilationResult.Status != AssetProcessingResult.CompilationStatus.Success || string.IsNullOrEmpty(mixins))
				return cssCompilationResult;

			var css = cssCompilationResult.Result.Substring(cssCompilationResult.Result.IndexOf(lessCompilerSeparatorColorRed) + lessCompilerSeparatorColorRed.Length);
			return new AssetProcessingResult(AssetProcessingResult.CompilationStatus.Success, css);
		}
	}
}
