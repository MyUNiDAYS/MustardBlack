using System.Net;

namespace MustardBlack.Tests.ModRewrite.RewriteRuleOnly.Success
{
	sealed class RegexPathWithCaptureGroups : SuccessfulRedirectModRewriteSpecification
	{
		protected override string Rules => "RewriteRule foo-([a-z]+) /baz-$1";

		protected override string RequestUrl => "https://www.unidays.test/foo-bar";

		protected override string ExpectedRedirectUrl => "https://www.unidays.test/baz-bar";
		protected override HttpStatusCode ExpectedStatusCode => HttpStatusCode.Redirect;
	}
}
