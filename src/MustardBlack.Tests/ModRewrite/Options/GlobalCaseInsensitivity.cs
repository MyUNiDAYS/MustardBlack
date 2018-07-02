using System.Net;

namespace MustardBlack.Tests.ModRewrite.Options
{
	public class GlobalCaseInsensitivity : SuccessfulRedirectModRewriteSpecification
	{
		protected override string Rules => @"

# Shouldnt be affected by the RewriteOptions
RewriteCond %{HTTP_HOST} UNiDAYS
RewriteRule .* /foo

# Shouldnt be affected by the RewriteOptions
RewriteRule BAR /foo

RewriteOptions nocase

# Should be affected by the RewriteOptions
RewriteRule BAR /baz

# Shouldnt get here
RewriteRule bar /boz

";

		protected override string RequestUrl => "https://unidays.test/bar";
		protected override string ExpectedRedirectUrl => "https://unidays.test/baz";
		protected override HttpStatusCode ExpectedStatusCode => HttpStatusCode.Found;
	}
}
