using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using MustardBlack.Assets.Babel;
using MustardBlack.Assets.Css;
using MustardBlack.Assets.Css.Less;
using MustardBlack.Assets.Css.Sass;
using MustardBlack.Assets.Javascript;
using MustardBlack.Assets.YuiCompressor;

namespace MustardBlack.Assets.Build
{
	sealed class Program
	{
		static void Main(string[] args)
		{
			try
			{
				Run(args);
			}
			catch (Exception e)
			{
				while (e != null)
				{
					Console.WriteLine(e.Message);
					Console.WriteLine(e.StackTrace);
					e = e.InnerException;
				}

				Environment.Exit(-1);
			}
		}

		static void Run(string[] args)
		{
			var path = args[0].Replace("/", "\\");
			var type = args[1];
			var outPath = args[2].Replace("/", "\\");

			var options = args.Length >= 4 ? args[3] : null;

			var assetLoader = new AssetLoader(new FakeFileSystem());

			string asset = null;
			string assetFormat = null;

			if (type == "css")
			{
				asset = ProcessCss(asset, assetLoader, path);
				assetFormat = "css";
			}
			else if (type == "js")
			{
				asset = ProcessJs(options, assetLoader, path);
				assetFormat = "js";
			}

			if (string.IsNullOrEmpty(asset))
				return;

			var resourceIntegrityCheck = ComputeSha256Hash(asset);
			Console.Write(resourceIntegrityCheck);

			var outFile = Path.Combine(outPath, resourceIntegrityCheck + '.' + assetFormat);
			File.WriteAllText(outFile, asset);
		}

		static string ProcessJs(string options, IAssetLoader assetLoader, string path)
		{
			var opts = options.Split(':');

			var assets = assetLoader.GetAssets(path, AreaJavascriptHandler.FileMatch);
			if (!assets.Any())
				return null;

			string result;

			IJavascriptPreprocessor javascriptPreprocessor;
			if (opts[0] == "babel")
			{
				javascriptPreprocessor = new BabelJavascriptPreprocessor(opts.Contains("sourcemaps"));
				result = javascriptPreprocessor.Process(assets);

				if (opts.Contains("minify"))
				{
					javascriptPreprocessor = new YuiJavascriptPreprocessor();
					result = javascriptPreprocessor.Process(new [] { new AssetContent("<babel-output>", result) });
				}
			}
			else
			{
				javascriptPreprocessor = new YuiJavascriptPreprocessor();
				result = javascriptPreprocessor.Process(assets);
			}

			return result;
		}

		static string ProcessCss(string asset, AssetLoader assetLoader, string path)
		{
			var cssPreprocessors = new ICssPreprocessor[] {new LessCssPreprocessor(), new SassCssPreprocessor()};

			foreach (var cssPreprocessor in cssPreprocessors)
			{
				asset = assetLoader.GetAsset(path, cssPreprocessor.FileMatch);
				if (string.IsNullOrWhiteSpace(asset))
					continue;

				var result = cssPreprocessor.Process(asset);

				if (result.Status == AssetProcessingResult.CompilationStatus.Skipped)
					continue;

				if (result.Status == AssetProcessingResult.CompilationStatus.Failure)
				{
					Console.WriteLine(result.Message);
					Environment.Exit(-1);
				}

				if (result.Status == AssetProcessingResult.CompilationStatus.Success)
				{
					asset = result.Result;
					break;
				}
			}

			return asset;
		}

		static string ComputeSha256Hash(string input)
		{
			using (var sha256Hash = SHA256.Create())
			{
				var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
				return "sha256-" + Convert.ToBase64String(bytes, Base64FormattingOptions.None).Replace('=', '.').Replace('/', '_');
			}
		}
	}
}
