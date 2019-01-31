using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using dotless.Core;
using dotless.Core.Loggers;
using dotless.Core.Parser;
using Serilog;
using ILogger = Serilog.ILogger;

namespace MustardBlack.Assets.Css.Less
{
	public static class LessCompiler
	{
		static readonly ILogger log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

		public static string Wrap(string less, string selector)
		{
			var builder = new StringBuilder(selector)
				.Append(" {")
				.AppendLine(less)
				.AppendLine("}");

			return builder.ToString();
		}

		[Obsolete("Use TryCompile instead, which lets you detect success or failure of the compilation")]
		public static string Compile(string less, string mixins = null, bool outputErrors = true)
		{
			var result = TryCompile(less, mixins);

			if (result.Status == AssetProcessingResult.CompilationStatus.Success)
				return result.Result;

			if (outputErrors)
				return result.Message;

			return null;
		}

		public static AssetProcessingResult TryCompile(string less, string mixins = null)
		{
			if (string.IsNullOrWhiteSpace(less))
				return new AssetProcessingResult(AssetProcessingResult.CompilationStatus.Success);

			var lastImportIndex = less.LastIndexOf("@import", StringComparison.OrdinalIgnoreCase);

			if (lastImportIndex == -1)
				return DoCompile(mixins + '\n' + less); // added whitespace to avoid another compiler issue

			var endOfImports = less.IndexOf(';', lastImportIndex);

			if (endOfImports == -1)
				return new AssetProcessingResult(AssetProcessingResult.CompilationStatus.Failure, null, "Cannot find `;` terminator for @import");

			var result = DoCompile(mixins + less.Substring(endOfImports + 1));
			if (result.Status == AssetProcessingResult.CompilationStatus.Success)
				result = new AssetProcessingResult(AssetProcessingResult.CompilationStatus.Success, less.Substring(0, endOfImports + 1) + result.Result);

			return result;
		}
		
		static AssetProcessingResult DoCompile(string less)
		{
			// LessEngine is not threadsafe, dont make it static
			var logger = new StringLessLogger(LogLevel.Error);
			var lessEngine = new LessEngine(new Parser(1), logger, true, false);

			try
			{
				var css = lessEngine.TransformToCss(less, null);
				if (logger.HasMessages)
					return new AssetProcessingResult(AssetProcessingResult.CompilationStatus.Failure, null, logger.Dump());

				return new AssetProcessingResult(AssetProcessingResult.CompilationStatus.Success, css);
			}
			catch (Exception e)
			{
				log.Error(e, "Error compiling less `{less}`", less);

				if(Debugger.IsAttached) Debugger.Break();

				return new AssetProcessingResult(AssetProcessingResult.CompilationStatus.Failure, null, e.Message);
			}
		}
		
	}
}
