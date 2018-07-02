using System.Net;

namespace MustardBlack.Tests.ModRewrite.RewriteRuleOnly.Success.QueryStrings
{
	sealed class RequestQSAndResponseQS : SuccessfulRedirectModRewriteSpecification
	{
		protected override string Rules => "RewriteRule foo-bar /baz?bar=baz";

		protected override string RequestUrl => "https://www.unidays.test/foo-bar?foo=bar";

		protected override string ExpectedRedirectUrl => "https://www.unidays.test/baz?bar=baz";
		protected override HttpStatusCode ExpectedStatusCode => HttpStatusCode.Redirect;
	}
}
