using System.Net;

namespace MustardBlack.Tests.ModRewrite.RewriteCondAndRule.Success
{
	sealed class NegatedCondition : SuccessfulRedirectModRewriteSpecification
	{
		protected override string Rules => @"
RewriteCond %{REQUEST_URI} !quux
RewriteRule .* /baz";

		protected override string RequestUrl => "https://www.unidays.test/foo-bar";

		protected override string ExpectedRedirectUrl => "https://www.unidays.test/baz";
		protected override HttpStatusCode ExpectedStatusCode => HttpStatusCode.Redirect;
	}
}
