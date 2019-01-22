using MustardBlack.Assets.Javascript;
using Yahoo.Yui.Compressor;

namespace MustardBlack.Assets.YuiCompressor
{
	public sealed class YuiJavascriptCompressor : IJavascriptCompressor
	{
		public string Compress(string input)
		{
			var javaScriptCompressor = new JavaScriptCompressor();
			return javaScriptCompressor.Compress(input);
		}
	}
}
