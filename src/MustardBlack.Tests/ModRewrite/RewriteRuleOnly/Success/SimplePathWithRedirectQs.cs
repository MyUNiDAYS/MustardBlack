using System.Net;

namespace MustardBlack.Tests.ModRewrite.RewriteRuleOnly.Success
{
	sealed class SimplePathWithRedirectQs : SuccessfulRedirectModRewriteSpecification
	{
		protected override string Rules => "RewriteRule foo-bar /baz?foo=bar";

		protected override string RequestUrl => "https://www.unidays.test/foo-bar?bar=baz";

		protected override string ExpectedRedirectUrl => "https://www.unidays.test/baz?foo=bar";
		protected override HttpStatusCode ExpectedStatusCode => HttpStatusCode.Redirect;
	}
}
