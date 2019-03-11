using System.Collections.Generic;
using System.Text;

namespace MustardBlack.Assets.Javascript
{
	public sealed class NullJavascriptPreprocessor : IJavascriptPreprocessor
	{
		public string Process(IEnumerable<AssetContent> assets)
		{
			var builder = new StringBuilder();
			foreach (var asset in assets)
				builder.AppendLine(asset.Contents);

			return builder.ToString();
		}
	}
}