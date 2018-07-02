using System.Net;

namespace MustardBlack.Tests.ModRewrite.RewriteRuleOnly.Success
{
	sealed class LeadingSlash : SuccessfulRedirectModRewriteSpecification
	{
		protected override string Rules => "RewriteRule ^foo-bar$ /baz";

		protected override string RequestUrl => "https://www.unidays.test/foo-bar";

		protected override string ExpectedRedirectUrl => "https://www.unidays.test/baz";
		protected override HttpStatusCode ExpectedStatusCode => HttpStatusCode.Redirect;
	}
}
