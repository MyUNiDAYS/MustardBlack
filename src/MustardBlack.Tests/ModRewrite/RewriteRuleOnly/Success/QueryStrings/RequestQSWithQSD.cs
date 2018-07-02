using System.Net;

namespace MustardBlack.Tests.ModRewrite.RewriteRuleOnly.Success.QueryStrings
{
	/// <summary>
	/// QSA Should trump existing QS values
	/// </summary>
	sealed class RequestQSWithQSD : SuccessfulRedirectModRewriteSpecification
	{
		protected override string Rules => "RewriteRule foo-bar /baz [QSD]";

		protected override string RequestUrl => "https://www.unidays.test/foo-bar?foo=bar";

		protected override string ExpectedRedirectUrl => "https://www.unidays.test/baz";
		protected override HttpStatusCode ExpectedStatusCode => HttpStatusCode.Redirect;
	}
}
