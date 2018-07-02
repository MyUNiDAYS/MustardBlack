using System.Collections.Generic;

namespace MustardBlack.ModRewrite
{
	public sealed class Ruleset
	{
		public List<RewriteCond> Conditions { get; set; }
		public RewriteRule Rule { get; set; }

		public Ruleset()
		{
			this.Conditions = new List<RewriteCond>();
		}
	}
}
