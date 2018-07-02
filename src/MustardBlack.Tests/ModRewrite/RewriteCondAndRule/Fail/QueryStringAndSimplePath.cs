using System.Net;

namespace MustardBlack.Tests.ModRewrite.RewriteCondAndRule.Fail
{
	sealed class QueryStringAndSimplePath : FailureModRewriteSpecification
	{
		protected override string Rules => @"
RewriteCond %{QUERY_STRING} ^?i-wont-match=
RewriteRule foo-bar /baz";

		protected override string RequestUrl => "https://www.unidays.test/foo-bar?foo=bar";
	}
}
