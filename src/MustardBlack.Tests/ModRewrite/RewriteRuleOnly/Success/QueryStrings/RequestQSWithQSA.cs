using System.Net;

namespace MustardBlack.Tests.ModRewrite.RewriteRuleOnly.Success.QueryStrings
{
	sealed class RequestQSWithQSA : SuccessfulRedirectModRewriteSpecification
	{
		protected override string Rules => "RewriteRule foo-bar /baz [QSA]";

		protected override string RequestUrl => "https://www.unidays.test/foo-bar?foo=bar";

		protected override string ExpectedRedirectUrl => "https://www.unidays.test/baz?foo=bar";
		protected override HttpStatusCode ExpectedStatusCode => HttpStatusCode.Redirect;
	}
}
