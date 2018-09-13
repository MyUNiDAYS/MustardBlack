using System.Collections.Generic;
using System.Reflection;
using MustardBlack.Hosting;
using MustardBlack.Results;
using Serilog;

namespace MustardBlack.ModRewrite
{
	public static class Engine
	{
		static readonly ILogger log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

		public static IResult Execute(IEnumerable<Ruleset> rulesets, IRequest request)
		{
			var ruleIndex = 0;

			foreach (var ruleset in rulesets)
			{
				var conditionArgs = new List<string>();

				var conditionsMet = true;

				log.Debug("ModRewrite: Rulset {index}: Evaluating, it {conditions} condition(s)", ruleIndex, ruleset.Conditions.Count);

				for (var conditionIndex = 0; conditionIndex < ruleset.Conditions.Count; conditionIndex++)
				{
					var condition = ruleset.Conditions[conditionIndex];
					
					var match = condition.Matches(request);
					if (match.Success && !condition.Negate)
					{
						log.Debug("ModRewrite: Rulset {index}, Condition {conditionIndex}: {condition}: Matched, proceeding", ruleIndex, conditionIndex, condition.ToString());

						for (var i = 1; i < match.Groups.Count; i++)
							conditionArgs.Add(match.Groups[i].Value);
						continue;
					}

					if (!match.Success && condition.Negate)
					{
						log.Debug("ModRewrite: Rulset {index}, Condition {conditionIndex}: {condition}: Negated Match, proceeding", ruleIndex, conditionIndex, condition.ToString());
						continue;
					}

					log.Debug("ModRewrite: Rulset {index}, Condition {conditionIndex}: {condition}: Not Matched, skipping ruleset", ruleIndex, conditionIndex, condition.ToString());

					conditionsMet = false;
					break;
				}

				if (conditionsMet)
				{
					log.Debug("ModRewrite: Rulset {index}, All conditions met, evaluating RewriteRule {rewriteRule}", ruleIndex, ruleset.Rule.Source);

					var result = ruleset.Rule.Execute(request, conditionArgs);
					if (result != null)
					{
						log.Debug("ModRewrite: Rulset {index}, RewriteRule Matched, returning result", ruleIndex);

						return result;
					}

					log.Debug("ModRewrite: Rulset {index}, RewriteRule Not Matched, skipping ruleset", ruleIndex);
				}

				ruleIndex++;
			}

			return null;
		}
	}
}
