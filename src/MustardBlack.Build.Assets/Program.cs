using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using MustardBlack.Assets;
using MustardBlack.Assets.Css;
using MustardBlack.Assets.Javascript;

namespace MustardBlack.Build.Assets
{
	sealed class Program
	{
		static void Main(string[] args)
		{
			var path = args[0].Replace("/", "\\");
			var type = args[1];
			var outPath = args[2].Replace("/", "\\");

			var assetLoader = new AssetLoader(new FakeFileSystem());
			
			// load asset
			var assetFormat = (AssetFormat)Enum.Parse(typeof(AssetFormat), type, true);
			var asset = assetLoader.GetAsset(path, assetFormat);
			
			if (string.IsNullOrWhiteSpace(asset))
				return;

			if (assetFormat == AssetFormat.Css)
			{
				var lessCssPreprocessor = new LessCssPreprocessor();
				var result = lessCssPreprocessor.Process(asset);
				if (result.Status != AssetProcessingResult.CompilationStatus.Success)
				{
					Console.WriteLine(result.Message);
					Environment.Exit(-1);
				}

				asset = result.Result;
			}
			else if (assetFormat == AssetFormat.Js)
			{
				var yuiJavascriptCompressor = new YuiJavascriptCompressor();
				var result = yuiJavascriptCompressor.Compress(asset);
				// TODO: handle failure
				asset = result;
			}

			// calc md5 and output
			var md5 = CalculateMd5Hash(asset);
			Console.Write(md5);

			var outFile = Path.Combine(outPath, md5 + '.' + assetFormat.ToString().ToLowerInvariant());
			File.WriteAllText(outFile, asset);
		}

		static string CalculateMd5Hash(string input)
		{
			var md5 = MD5.Create();
			var inputBytes = Encoding.UTF8.GetBytes(input);
			var hash = md5.ComputeHash(inputBytes);

			return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
		}
	}
}
