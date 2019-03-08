using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using MustardBlack.Hosting;

namespace MustardBlack.Example.Areas.Example.Example
{
	public class ScriptTagHelper : TagHelper
	{
		readonly IResponse response;

		public ScriptTagHelper(IResponse response)
		{
			this.response = response;
		}
		
		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			var cspShaAttribute = context.AllAttributes["csp-sha"];
			if (cspShaAttribute == null)
				return;

			output.Attributes.Remove(cspShaAttribute);

			var tagContent = await output.GetChildContentAsync();
			var content = tagContent.GetContent();

			// trim this, cant work out how browsers handle it, no may as well remove it and avoid the issue
			content = content.Trim();

			var sha = ComputeSha256Hash(content);

			output.Content.SetHtmlContent(content);

			//this.response.Headers.Set("Content-Security-Policy", "script-src 'sha256-" + sha + "'");
		}

		static string ComputeSha256Hash(string rawData)
		{
			using (var sha256Hash = SHA256.Create())
			{
				var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
				return Convert.ToBase64String(bytes);
			}
		}
	}
}