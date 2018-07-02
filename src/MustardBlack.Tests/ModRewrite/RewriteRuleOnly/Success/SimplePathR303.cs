using System.Net;

namespace MustardBlack.Tests.ModRewrite.RewriteRuleOnly.Success
{
	sealed class SimplePathR303 : SuccessfulRedirectModRewriteSpecification
	{
		protected override string Rules => "RewriteRule foo-bar /baz [R=303]";

		protected override string RequestUrl => "https://www.unidays.test/foo-bar";

		protected override string ExpectedRedirectUrl => "https://www.unidays.test/baz";
		protected override HttpStatusCode ExpectedStatusCode => HttpStatusCode.SeeOther;
	}
}
