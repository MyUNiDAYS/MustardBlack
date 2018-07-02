using System.Net;

namespace MustardBlack.Tests.ModRewrite.RewriteCondAndRule.Success
{
	sealed class HostConditionCaptureGroupAndSimplePath : SuccessfulRedirectModRewriteSpecification
	{
		protected override string Rules => @"
RewriteCond %{HTTP_Host} unidays\.([a-z]+)
RewriteRule foo-bar /baz-%1";

		protected override string RequestUrl => "https://www.unidays.test/foo-bar";

		protected override string ExpectedRedirectUrl => "https://www.unidays.test/baz-test";
		protected override HttpStatusCode ExpectedStatusCode => HttpStatusCode.Redirect;
	}
}
