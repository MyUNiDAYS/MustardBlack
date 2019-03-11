using System.Collections.Generic;
using System.Text;
using MustardBlack.Assets.Javascript;
using Yahoo.Yui.Compressor;

namespace MustardBlack.Assets.YuiCompressor
{
	public sealed class YuiJavascriptPreprocessor : IJavascriptPreprocessor
	{
		public string Process(IEnumerable<AssetContent> assets)
		{
			var builder = new StringBuilder();
			foreach (var asset in assets)
				builder.AppendLine(asset.Contents);

			var javaScriptCompressor = new JavaScriptCompressor{Encoding = Encoding.UTF8};
			return javaScriptCompressor.Compress(builder.ToString());
		}
	}
}
