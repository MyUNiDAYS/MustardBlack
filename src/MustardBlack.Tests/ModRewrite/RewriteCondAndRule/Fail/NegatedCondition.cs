namespace MustardBlack.Tests.ModRewrite.RewriteCondAndRule.Fail
{
	sealed class NegatedCondition : FailureModRewriteSpecification
	{
		protected override string Rules => @"
RewriteCond %{REQUEST_URI} !foo-bar
RewriteRule .* /baz";

		protected override string RequestUrl => "https://www.unidays.test/foo-bar";
	}
}
