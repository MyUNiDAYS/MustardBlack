using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using MustardBlack.Hosting;

namespace MustardBlack.ModRewrite
{
	[DebuggerDisplay("source")]
	public sealed class RewriteCond
	{
		public RewriteCondFlags Flags { get; set; }
		public string TestString { get; set; }
		public Regex Condition { get; set; }
		string source;

		[Flags]
		public enum RewriteCondFlags
		{
			CaseInsensitive = 1
		}

		public static RewriteCond Parse(string line, string[] lineTokens, RewriteOptions options)
		{
			var flags = ParseFlags(lineTokens);

			var regexOptions = flags.HasFlag(RewriteCondFlags.CaseInsensitive) || options.GlobalCaseInsensitivity ? RegexOptions.IgnoreCase : RegexOptions.None;
			var condition = new Regex(lineTokens[2], regexOptions, TimeSpan.FromMilliseconds(500));

			return new RewriteCond
			{
				TestString = lineTokens[1],
				Flags = flags,
				Condition = condition,
				source = line
			};
		}

		static RewriteCondFlags ParseFlags(IReadOnlyList<string> lineTokens)
		{
			var flags = (RewriteCondFlags) 0;
			if (lineTokens.Count == 4)
			{
				var flagsStr = lineTokens[3].ToLowerInvariant();
				flagsStr = flagsStr.Trim('[', ']');
				var flagsArray = flagsStr.Split(',');

				foreach (var flag in flagsArray)
				{
					switch (flag)
					{
						case "nc":
						case "nocase":
							flags |= RewriteCondFlags.CaseInsensitive;
							break;
					}
				}
			}

			return flags;
		}

		public Match Matches(IRequest request)
		{
			switch (this.TestString.ToLowerInvariant())
			{
				// Support mod_rewrite and isapi_rewrite syntax
				case "%{http_host}":
				case "%{http:host}":
					return this.Condition.Match(request.Url.Host);
				case "%{request_uri}":
					return this.Condition.Match(request.Url.Path);
				case "%{query_string}":
					return this.Condition.Match(request.Url.QueryString);
			}

			return Match.Empty;
		}

		public override string ToString()
		{
			return this.source;
		}
	}
}
