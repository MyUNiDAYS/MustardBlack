using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
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
			var path = args[0].Replace("/", "\\");
			var type = args[1];
			var outPath = args[2].Replace("/", "\\");

			var assetLoader = new AssetLoader(new FakeFileSystem());

			string asset = null;
			string assetFormat = null;

			if (type == "css")
			{
				var cssPreprocessors = new ICssPreprocessor[] { new LessCssPreprocessor(), new SassCssPreprocessor() };

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
						assetFormat = "css";
						break;
					}
				}
				
			}
			else if (type == "js")
			{
				var yuiJavascriptCompressor = new YuiJavascriptCompressor();

				asset = assetLoader.GetAsset(path, AreaJavascriptHandler.FileMatch);
				if (string.IsNullOrWhiteSpace(asset))
					return;

				var result = yuiJavascriptCompressor.Compress(asset);
				// TODO: handle failure
				asset = result;
				assetFormat = "js";
			}

			var resourceIntegrityCheck = ComputeSha256Hash(asset);
			Console.Write(resourceIntegrityCheck);

			var outFile = Path.Combine(outPath, resourceIntegrityCheck + '.' + assetFormat);
			File.WriteAllText(outFile, asset);
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
