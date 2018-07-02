namespace MustardBlack.Tests.ModRewrite.RewriteCondAndRule.Fail
{
	sealed class HostConditionAndSimplePath : FailureModRewriteSpecification
	{
		protected override string Rules => @"
RewriteCond %{HTTP_Host} i-dont-match
RewriteRule foo-bar /baz";

		protected override string RequestUrl => "https://www.unidays.test/foo-bar";
		
	}
}
