namespace MustardBlack.Hosting.AspNetCore
{
	public sealed class HeaderCloakingModule : IHttpModule
	{
		readonly string[] headersToCloak;

		public HeaderCloakingModule()
		{
			this.headersToCloak = new[]
				                      {
					                      "Server"
				                      };
		}

		public void Init(HttpApplication context)
		{
			context.PreSendRequestHeaders += this.OnPreSendRequestHeaders;
		}

		void OnPreSendRequestHeaders(object sender, System.EventArgs e)
		{
			foreach (var header in this.headersToCloak)
				HttpContext.Current.Response.Headers.Remove(header);

			//if(!HttpContext.Current.Request.Url.Host.EndsWith(".dev"))
				//HttpContext.Current.Response.Headers.Set("Strict-Transport-Security", "max-age=31536000");

			//HttpContext.Current.Response.Headers.Set("X-Content-Type-Options", "nosniff");

			// TODO: implement: http://www.html5rocks.com/en/tutorials/security/content-security-policy/
			//HttpContext.Current.Response.Headers.Set("Content-Security-Policy", "nosniff");
		}

		public void Dispose()
		{
		}
	}
}