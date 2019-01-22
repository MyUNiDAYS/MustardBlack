using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using MustardBlack.Assets.Css;
using MustardBlack.Assets.Javascript;
using MustardBlack.Assets.Less;
using MustardBlack.Assets.Sass;
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

			if (type == "less" || type == "css" || type == "sass")
			{
				ICssPreprocessor cssPreprocessor;

				if (type == "sass")
					cssPreprocessor = new SassCssPreprocessor();
				else
					cssPreprocessor = new LessCssPreprocessor();

				asset = assetLoader.GetAsset(path, cssPreprocessor.FileMatch);
				if (string.IsNullOrWhiteSpace(asset))
					return;

				var result = cssPreprocessor.Process(asset);
				if (result.Status != AssetProcessingResult.CompilationStatus.Success)
				{
					Console.WriteLine(result.Message);
					Environment.Exit(-1);
				}

				asset = result.Result;
				assetFormat = "css;";
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
