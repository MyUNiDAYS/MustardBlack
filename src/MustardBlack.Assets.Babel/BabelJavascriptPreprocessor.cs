using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using JavaScriptEngineSwitcher.Core;
using JavaScriptEngineSwitcher.Msie;
using MustardBlack.Assets.Javascript;
using React;
using SourcemapToolkit.SourcemapParser;

namespace MustardBlack.Assets.Babel
{
	public sealed class BabelJavascriptPreprocessor : IJavascriptPreprocessor
	{
		IReactEnvironment reactEnvironment;
		string babelConfig;

		public BabelJavascriptPreprocessor()
		{
			Initializer.Initialize(r => r.AsSingleton());

			var container = AssemblyRegistration.Container;
			container.Register<ICache, NullCache>();
			container.Register<IFileSystem, SimpleFileSystem>();
			ReactSiteConfiguration.Configuration.SetReuseJavaScriptEngines(true);

			JsEngineSwitcher.Current.DefaultEngineName = MsieJsEngine.EngineName;
			JsEngineSwitcher.Current.EngineFactories.AddMsie();
			
			this.reactEnvironment = ReactEnvironment.Current;
			this.reactEnvironment.Configuration.BabelConfig = new BabelConfig();

			this.babelConfig = this.reactEnvironment.Configuration.BabelConfig.Serialize();
		}
		
		public string Process(IEnumerable<AssetContent> assets)
		{
			var results = assets.Select(a => new { asset = a, result = this.reactEnvironment.ExecuteWithBabel<JavaScriptWithSourceMap>("ReactNET_transform_sourcemap", a.Contents, babelConfig, a.FullPath) }).ToArray();

			var offset = 0;
			var map = new SourcemapToolkit.SourcemapParser.SourceMap();
			var sourceMapParser = new SourceMapParser();
			var outputBuilder = new StringBuilder();

			foreach (var result in results)
			{
				var json = result.result.SourceMap.ToJson();
				using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
				{
					using (var streamReader = new StreamReader(memoryStream))
					{
						var sourceMap = sourceMapParser.ParseSourceMap(streamReader);
						var maxLine = 0;

						foreach (var name in sourceMap.Names)
							map.Names.Add(name);

						map.Sources.Add(result.asset.FullPath);

						foreach (var mappingEntry in sourceMap.ParsedMappings)
						{
							// TODO: how should we really handle this? How can you have mappingentries with no originalsourceposition?
							if (mappingEntry.OriginalSourcePosition == null)
								continue;

							map.ParsedMappings.Add(mappingEntry);
							
							mappingEntry.OriginalFileName = result.asset.FullPath;
							mappingEntry.GeneratedSourcePosition.ZeroBasedLineNumber += offset;
						}

						offset += result.asset.Contents.Split('\n').Length - 1;
					}
				}

				outputBuilder.AppendLine(result.result.Code);
			}


			var sourceMapGenerator = new SourceMapGenerator();
			var generateSourceMapInlineComment = sourceMapGenerator.GenerateSourceMapInlineComment(map);

			outputBuilder.AppendLine(generateSourceMapInlineComment);

			return outputBuilder.ToString();
		}
	}
}