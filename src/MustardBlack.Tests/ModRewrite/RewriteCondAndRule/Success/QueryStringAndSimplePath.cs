using System.Net;

namespace MustardBlack.Tests.ModRewrite.RewriteCondAndRule.Success
{
	sealed class QueryStringAndSimplePath : SuccessfulRedirectModRewriteSpecification
	{
		protected override string Rules => @"
RewriteCond %{QUERY_STRING} ^?foo=
RewriteRule foo-bar /baz";

		protected override string RequestUrl => "https://www.unidays.test/foo-bar?foo=bar";

		protected override string ExpectedRedirectUrl => "https://www.unidays.test/baz?foo=bar";
		protected override HttpStatusCode ExpectedStatusCode => HttpStatusCode.Redirect;
	}
}
