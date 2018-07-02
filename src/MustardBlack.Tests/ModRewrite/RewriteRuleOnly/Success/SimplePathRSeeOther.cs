using System.Net;

namespace MustardBlack.Tests.ModRewrite.RewriteRuleOnly.Success
{
	sealed class SimplePathRSeeOther : SuccessfulRedirectModRewriteSpecification
	{
		protected override string Rules => "RewriteRule foo-bar /baz [seeother]";

		protected override string RequestUrl => "https://www.unidays.test/foo-bar";

		protected override string ExpectedRedirectUrl => "https://www.unidays.test/baz";
		protected override HttpStatusCode ExpectedStatusCode => HttpStatusCode.SeeOther;
	}
}
