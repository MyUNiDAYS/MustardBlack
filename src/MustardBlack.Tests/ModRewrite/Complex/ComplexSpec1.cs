using System.Net;

namespace MustardBlack.Tests.ModRewrite.Complex
{
	sealed class ComplexSpec1 : SuccessfulRedirectModRewriteSpecification
	{
		protected override string Rules => @"
RewriteCond %{HTTP_Host} ([a-z]+\.UNiDAYS) [nocase]
RewriteCond %{REQUEST_URI} (fo+)-bar
RewriteRule .* /baz-%2/%1";

		protected override string RequestUrl => "https://www.unidays.test/foooo-bar";

		protected override string ExpectedRedirectUrl => "https://www.unidays.test/baz-foooo/www.unidays";
		protected override HttpStatusCode ExpectedStatusCode => HttpStatusCode.Redirect;
	}
}
