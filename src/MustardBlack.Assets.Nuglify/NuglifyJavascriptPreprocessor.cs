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
			    var nuggled = Uglify.Js(javascriptString);

			    if (!nuggled.HasErrors)
				    return nuggled.Code;

			    foreach (var error in nuggled.Errors)
			    {
				    Console.WriteLine($"Nuglify Compilation Error: {error.Message} (lines: {error.StartLine}-{error.EndLine}): File: {assetPath}");
			    }
				
			    throw new Exception("Nuglify failed to process some JavaScript: See above for detail");
		    }
		    catch (Exception e)
		    {
			    Console.WriteLine($"Nuglify Compilation Error: {e.Message}: File: {assetPath}");
			    throw;
		    }
	    }
    }
}
