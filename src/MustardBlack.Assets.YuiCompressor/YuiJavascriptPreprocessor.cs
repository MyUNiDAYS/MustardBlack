using System;
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
            var javaScriptCompressor = new JavaScriptCompressor {Encoding = Encoding.UTF8};

            //check all assets
            foreach (var asset in assets)
                CompressJavascript(javaScriptCompressor, asset.Contents, asset.Path);

            var builder = new StringBuilder();
            foreach (var asset in assets)
                builder.AppendLine(asset.Contents);

            return CompressJavascript(javaScriptCompressor, builder.ToString(), null);
        }

        static string CompressJavascript(JavaScriptCompressor javaScriptCompressor, string javascriptString, string assetPath)
        {
            try
            {
                return javaScriptCompressor.Compress(javascriptString);
            }
            catch (EcmaScript.NET.EcmaScriptRuntimeException e)
            {
                Console.WriteLine($"YUI Compression Error: '{e.Message}'\nJavascript Syntax Error: '{e.LineSource}'\nLine {e.LineNumber}\nFile: {assetPath}");
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine($"YUI Compression Error: {e.Message}: File: {assetPath}");
                throw;
            }
        }
    }
}