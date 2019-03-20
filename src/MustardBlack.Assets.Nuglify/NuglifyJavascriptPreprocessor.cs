using System;
using System.Collections.Generic;
using System.Text;
using MustardBlack.Assets.Javascript;
using NUglify;

namespace MustardBlack.Assets.Nuglify
{
    public class NuglifyJavascriptPreprocessor : IJavascriptPreprocessor
    {
	    public string Process(IEnumerable<AssetContent> assets)
	    {
		    // Check that the individual assets compress in on their own.
		    foreach (var asset in assets)
			    CompressJavascript(asset.Contents, asset.Path);
		    
		    var builder = new StringBuilder();
		    foreach (var asset in assets)
			    builder.AppendLine(asset.Contents);
		    
		    return CompressJavascript(builder.ToString(), null);
	    }

	    static string CompressJavascript(string javascriptString, string assetPath)
	    {
		    try
		    {
			    return Uglify.Js(javascriptString).Code;
		    }
		    catch (Exception e)
		    {
			    Console.WriteLine($"Nuglify Compression Error: {e.Message}: File: {assetPath}");
			    throw;
		    }
	    }
    }
}
