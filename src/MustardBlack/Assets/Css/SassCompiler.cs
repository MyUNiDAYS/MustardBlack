using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using Serilog;
using ILogger = Serilog.ILogger;
using LibSass.Compiler.Options;

namespace MustardBlack.Assets.Css
{
	public static class SassCompiler
	{
		static readonly ILogger log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

		public static string Wrap(string sass, string selector)
		{
			var builder = new StringBuilder(selector)
				.Append(" {")
				.AppendLine(sass)
				.AppendLine("}");

			return builder.ToString();
		}

		[Obsolete("Use TryCompile instead, which lets you detect success or failure of the compilation")]
		public static string Compile(string sass, string mixins = null, bool outputErrors = true)
		{
			var result = TryCompile(sass, mixins);

			if (result.Status == AssetProcessingResult.CompilationStatus.Success)
				return result.Result;

			if (outputErrors)
				return result.Message;

			return null;
		}

		public static AssetProcessingResult TryCompile(string sass, string mixins = null)
		{
			if (string.IsNullOrWhiteSpace(sass))
				return new AssetProcessingResult(AssetProcessingResult.CompilationStatus.Success, string.Empty);

			var lastImportIndex = sass.LastIndexOf("@import", StringComparison.OrdinalIgnoreCase);

			if (lastImportIndex == -1)
				return DoCompile(mixins + '\n' + sass); // added whitespace to avoid another compiler issue

			var endOfImports = sass.IndexOf(';', lastImportIndex);

			if (endOfImports == -1)
				return new AssetProcessingResult(AssetProcessingResult.CompilationStatus.Failure, null, "Cannot find `;` terminator for @import");

			var result = DoCompile(mixins + sass.Substring(endOfImports + 1));
			if (result.Status == AssetProcessingResult.CompilationStatus.Success)
				result = new AssetProcessingResult(AssetProcessingResult.CompilationStatus.Success, sass.Substring(0, endOfImports + 1) + result.Result);

			return result;
		}
		
		static AssetProcessingResult DoCompile(string sass)
		{
			var sassOptions = new SassOptions
			{
				Data = sass,
				OutputStyle = SassOutputStyle.Compressed,
				
			};
			var sassCompiler = new LibSass.Compiler.SassCompiler(sassOptions);
			
			try
			{
				var result = sassCompiler.Compile();
				if (result.ErrorStatus != 0)
					// TODO: enrich error dump
					return new AssetProcessingResult(AssetProcessingResult.CompilationStatus.Failure, null, result.ErrorMessage);

				return new AssetProcessingResult(AssetProcessingResult.CompilationStatus.Success, result.Output);
			}
			catch (Exception e)
			{
				log.Error(e, "Error compiling sass `{sass}`", sass);

				if(Debugger.IsAttached) Debugger.Break();

				return new AssetProcessingResult(AssetProcessingResult.CompilationStatus.Failure, null, e.Message);
			}
		}
		
	}
}
