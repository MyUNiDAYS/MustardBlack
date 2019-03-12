using System;
using System.Text.RegularExpressions;
using MustardBlack.Assets;
using MustardBlack.Assets.Babel;
using MustardBlack.ViewEngines.Razor.Build;

namespace MustardBlack.Tests.Babel
{
	public class BableJsPreprocessorSpecs : Specification
	{
		protected override void When()
		{
			var assetLoader = new AssetLoader(new TestFileSystem(AppDomain.CurrentDomain.BaseDirectory));
			var assetContents = assetLoader.GetAssets("~/Babel/scripts", new Regex(".js$"));
			var babelJavascriptPreprocessor = new BabelJavascriptPreprocessor(true);
			var process = babelJavascriptPreprocessor.Process(assetContents);
		}
//
//		[Then]
//		public void ShouldntBeFucked()
//		{
//			
//		}
	}
}