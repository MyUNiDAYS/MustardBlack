using System;
using System.Text;
using System.Text.RegularExpressions;
using MustardBlack.Assets.Css;

namespace MustardBlack.Assets.Less
{
	public sealed class LessCssPreprocessor : ICssPreprocessor
	{
		const string lessCompilerSeparatorColorRed = ".less-compiler-separator{color:red}";
		static readonly Regex fileMatch = new Regex(@"(\.less|\.css)$", RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1));

		public Regex FileMatch => fileMatch;

		public AssetProcessingResult Process(string input, string mixins = null)
		{
			if(string.IsNullOrWhiteSpace(input))
				return new AssetProcessingResult(AssetProcessingResult.CompilationStatus.Success, string.Empty);

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
