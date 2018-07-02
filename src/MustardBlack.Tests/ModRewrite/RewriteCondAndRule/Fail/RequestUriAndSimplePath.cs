using System.Net;

namespace MustardBlack.Tests.ModRewrite.RewriteCondAndRule.Fail
{
	sealed class RequestUriAndSimplePath : FailureModRewriteSpecification
	{
		protected override string Rules => @"
RewriteCond %{REQUEST_URI} i-wont-match
RewriteRule .* /baz";

		protected override string RequestUrl => "https://www.unidays.test/foo-bar";
	}
}
