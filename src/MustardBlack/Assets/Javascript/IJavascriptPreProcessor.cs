using System.Collections.Generic;

namespace MustardBlack.Assets.Javascript
{
	public interface IJavascriptPreprocessor
	{
		string Process(IEnumerable<AssetContent> assets);
	}
}
