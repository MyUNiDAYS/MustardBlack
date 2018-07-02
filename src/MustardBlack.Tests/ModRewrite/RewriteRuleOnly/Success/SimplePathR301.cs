using System.Net;

namespace MustardBlack.Tests.ModRewrite.RewriteRuleOnly.Success
{
	sealed class SimplePathR301 : SuccessfulRedirectModRewriteSpecification
	{
		protected override string Rules => "RewriteRule foo-bar /baz [R=301]";

		protected override string RequestUrl => "https://www.unidays.test/foo-bar";

		protected override string ExpectedRedirectUrl => "https://www.unidays.test/baz";
		protected override HttpStatusCode ExpectedStatusCode => HttpStatusCode.MovedPermanently;
	}
}
