using System.Net;

namespace MustardBlack.Tests.ModRewrite.RewriteCondAndRule.Success
{
	sealed class HostConditionAndSimplePath : SuccessfulRedirectModRewriteSpecification
	{
		protected override string Rules => @"
RewriteCond %{HTTP_Host} unidays
RewriteRule foo-bar /baz";

		protected override string RequestUrl => "https://www.unidays.test/foo-bar";

		protected override string ExpectedRedirectUrl => "https://www.unidays.test/baz";
		protected override HttpStatusCode ExpectedStatusCode => HttpStatusCode.Redirect;
	}
}
