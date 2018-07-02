using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using MustardBlack.Hosting;
using System.Net;
using System.Text;
using MustardBlack.Results;

namespace MustardBlack.ModRewrite
{
	[DebuggerDisplay("Source")]
	public sealed class RewriteRule
	{
		public RewriteRuleFlags Flags { get; private set; }
		public HttpStatusCode StatusCode { get; set; }
		public string RewrittenUrl { get; private set; }
		public Regex PathRegex { get; private set; }
		public string Source { get; private set; }
		public RewriteOptions Options { get; private set; }

		[Flags]
		public enum RewriteRuleFlags
		{
			CaseInsensitive = 1,
			QueryStringAppend = 2,
			QueryStringDrop = 4
		}

		public static RewriteRule Parse(string line, string[] lineTokens, RewriteOptions options)
		{
			var rewriteRule = new RewriteRule
			{
				RewrittenUrl = lineTokens[2],
				Source = line,
				Options = options,
				StatusCode = options.DefaultResponseStatusCode
			};

			ParseAndApplyFlags(lineTokens, rewriteRule);

			var regexOptions = rewriteRule.Flags.HasFlag(RewriteRuleFlags.CaseInsensitive) || options.GlobalCaseInsensitivity ? RegexOptions.IgnoreCase : RegexOptions.None;
			rewriteRule.PathRegex = new Regex(lineTokens[1], regexOptions, TimeSpan.FromMilliseconds(100));
			

			return rewriteRule;
		}

		static void ParseAndApplyFlags(string[] lineTokens, RewriteRule rule)
		{
			if (lineTokens.Length != 4)
				return;
			
			var flagStr = lineTokens[3].ToLowerInvariant();
			flagStr = flagStr.Trim('[', ']');
			var flagArray = flagStr.Split(',');

			foreach (var flag in flagArray)
			{
				switch (flag)
				{
					case "f":
					case "forbidden":
						rule.StatusCode = HttpStatusCode.Forbidden;
						continue;

					case "g":
					case "gone":
						rule.StatusCode = HttpStatusCode.Gone;
						continue;
				
					case "nc":
					case "nocase":
						rule.Flags |= RewriteRuleFlags.CaseInsensitive;
						continue;

					case "qsa":
					case "qsappend":
						rule.Flags |= RewriteRuleFlags.QueryStringAppend;
						continue;

					case "qsd":
					case "qsdrop":
						rule.Flags |= RewriteRuleFlags.QueryStringDrop;
						continue;
						
					case "permanent":
						rule.StatusCode = HttpStatusCode.MovedPermanently;
						continue;
						
					case "seeother":
						rule.StatusCode = HttpStatusCode.SeeOther;
						continue;
					
					case "temp":
						rule.StatusCode = HttpStatusCode.TemporaryRedirect;
						continue;
				}

				if (flag.StartsWith("r="))
					rule.StatusCode = (HttpStatusCode)int.Parse(flag.Substring(2));
			}
		}

		public IResult Execute(IRequest request, List<string> conditionArgs)
		{
			// remove the leading slash, because mod_rewrite contradicts itself
			var trimStart = request.Url.Path.TrimStart('/');

			var ruleMatch = this.PathRegex.Match(trimStart);
			if (!ruleMatch.Success)
				return null;

			if ((int)this.StatusCode < 300 || (int)this.StatusCode > 399)
				return new EmptyResult(this.StatusCode);
			
			// Redirect of some sort

			var newUrlBuilder = new StringBuilder(this.RewrittenUrl);
			for (var i = 0; i < conditionArgs.Count; i++)
				newUrlBuilder.Replace("%" + (i + 1), conditionArgs[i]);

			for (var i = 1; i < ruleMatch.Groups.Count; i++)
				newUrlBuilder.Replace("$" + i, ruleMatch.Groups[i].Value);

			var newUrl = new Url(newUrlBuilder.ToString());

			if (newUrl.QueryCollection.HasKeys())
			{
				if (this.Flags.HasFlag(RewriteRule.RewriteRuleFlags.QueryStringAppend))
				{
					foreach (var key in request.Url.QueryCollection.AllKeys)
						newUrl.QueryCollection[key] = request.Url.QueryCollection[key];
				}
			}
			else
			{
				if (!this.Flags.HasFlag(RewriteRule.RewriteRuleFlags.QueryStringDrop))
				{
					foreach (var key in request.Url.QueryCollection.AllKeys)
						newUrl.QueryCollection[key] = request.Url.QueryCollection[key];
				}
			}

			// Always redirect to an absolute url
			if (!newUrl.IsAbsolute)
			{
				var pathAndQuery = newUrl.PathAndQuery;
				newUrl = new Url(request.Url);
				newUrl.PathAndQuery = pathAndQuery;
			}
			
			return new RedirectResult(this.StatusCode, newUrl);
		}

		public override string ToString()
		{
			return this.Source;
		}
	}
}
