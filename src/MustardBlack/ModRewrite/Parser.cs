using System;
using System.Collections.Generic;

namespace MustardBlack.ModRewrite
{
	public static class Parser
	{
		public static IEnumerable<Ruleset> Parse(string rules)
		{
			var lines = rules.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

			var rulesets = new List<Ruleset>();

			var ruleset = new Ruleset();

			var options = new RewriteOptions();

			foreach (var line in lines)
			{
				try
				{
					var tokens = line.Trim().Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

					if (tokens[0] == "RewriteCond")
					{
						ruleset.Conditions.Add(RewriteCond.Parse(line, tokens, options));
					}
					else if (tokens[0] == "RewriteRule")
					{
						ruleset.Rule = RewriteRule.Parse(line, tokens, options);

						rulesets.Add(ruleset);
						ruleset = new Ruleset();
					}
					else if (tokens[0] == "RewriteOptions")
					{
						options = RewriteOptions.Parse(line, tokens);
					}
				}
				catch
				{
				}
			}

			return rulesets;
		}
	}
}
