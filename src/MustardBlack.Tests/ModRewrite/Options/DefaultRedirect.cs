using System.Net;

namespace MustardBlack.Tests.ModRewrite.Options
{
	public class DefaultRedirect : SuccessfulRedirectModRewriteSpecification
	{
		protected override string Rules => @"

RewriteOptions R=307
RewriteRule bar /baz

";

		protected override string RequestUrl => "https://unidays.test/bar";
		protected override string ExpectedRedirectUrl => "https://unidays.test/baz";
		protected override HttpStatusCode ExpectedStatusCode => HttpStatusCode.TemporaryRedirect;
	}
}
