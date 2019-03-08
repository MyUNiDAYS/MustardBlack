using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using JavaScriptEngineSwitcher.Core;
using JavaScriptEngineSwitcher.Msie;
using MustardBlack.Handlers;
using MustardBlack.Hosting;
using MustardBlack.Results;
using React;
using SourcemapToolkit.SourcemapParser;

namespace MustardBlack.Assets.Javascript
{
	/// <summary>
	/// Should always be mapped to handle "/{area}.js"
	/// </summary>
	public sealed class AreaJavascriptHandler : Handler
	{
		readonly IAssetLoader assetLoader;
		// Must end with ".js", but not ".test.js"
		public static readonly Regex FileMatch = new Regex(@".*(?<!test).js$", RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1));
		public AreaJavascriptHandler(IAssetLoader assetLoader)
		{
			this.assetLoader = assetLoader;
		}

		public IResult Get(IRequest request)
		{
			var area = request.Url.Path.Substring(1, request.Url.Path.IndexOf('.') - 1);
			var path = "~/areas/" + area + "/assets/scripts/";

			var assets = this.assetLoader.GetAssets(path, FileMatch);

			Initializer.Initialize(r => r.AsSingleton());


			var container = React.AssemblyRegistration.Container;
			container.Register<ICache, NullCache>();
			container.Register<React.IFileSystem, SimpleFileSystem>();
			ReactSiteConfiguration.Configuration.SetReuseJavaScriptEngines(true);

			JsEngineSwitcher.Current.DefaultEngineName = MsieJsEngine.EngineName;
			JsEngineSwitcher.Current.EngineFactories.AddMsie();

			var environment = ReactEnvironment.Current;

			var babelConfig = environment.Configuration.BabelConfig.Serialize();
			var results = assets.Select(a => new { asset = a, result = environment.ExecuteWithBabel<JavaScriptWithSourceMap>("ReactNET_transform_sourcemap", a.Contents, babelConfig, a.FullPath) }).ToArray();
			
			var offset = 0;
			var map = new SourcemapToolkit.SourcemapParser.SourceMap();
			map.ParsedMappings = new List<MappingEntry>();
			map.Names = new List<string>();
			map.Sources = new List<string>();

			var sourceMapParser = new SourceMapParser();

			var stringBuilder = new StringBuilder();

			foreach (var result in results)
			{
				var json = result.result.SourceMap.ToJson();
				using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
				{
					using (var streamReader = new StreamReader(memoryStream))
					{
						var sourceMap = sourceMapParser.ParseSourceMap(streamReader);
						var maxLine = 0;

						foreach(var name in sourceMap.Names)
							map.Names.Add(name);

						map.Sources.Add(result.asset.FullPath);

						foreach (var mappingEntry in sourceMap.ParsedMappings)
						{
							map.ParsedMappings.Add(mappingEntry);

							if (mappingEntry.OriginalSourcePosition.ZeroBasedLineNumber > maxLine)
								maxLine = mappingEntry.OriginalSourcePosition.ZeroBasedLineNumber;
							mappingEntry.OriginalFileName = result.asset.FullPath;
							mappingEntry.OriginalSourcePosition.ZeroBasedLineNumber += offset;
						}

						offset += maxLine;
					}
				}

				stringBuilder.AppendLine(result.result.Code);
			}


			var sourceMapGenerator = new SourceMapGenerator();
			var generateSourceMapInlineComment = sourceMapGenerator.GenerateSourceMapInlineComment(map);

			stringBuilder.AppendLine(generateSourceMapInlineComment);

			return new FileContentResult("application/javascript", Encoding.UTF8.GetBytes(stringBuilder.ToString()));
		}
	}
}
