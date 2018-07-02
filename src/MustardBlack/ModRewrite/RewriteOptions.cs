using System.Collections.Generic;
using System.Net;

namespace MustardBlack.ModRewrite
{
	public sealed class RewriteOptions
	{
		public bool GlobalCaseInsensitivity { get; private set; }
		public HttpStatusCode DefaultResponseStatusCode { get; set; }

		public RewriteOptions()
		{
			this.DefaultResponseStatusCode = HttpStatusCode.Found;
		}

		public static RewriteOptions Parse(string line, IEnumerable<string> lineTokens)
		{
			var options = new RewriteOptions();

			foreach (var token in lineTokens)
			{
				switch (token.ToLowerInvariant())
				{
					case "nc":
					case "nocase":
						options.GlobalCaseInsensitivity = true;
						break;

					case "r=301":
					case "permanent":
						options.DefaultResponseStatusCode = HttpStatusCode.MovedPermanently;
						break;

					case "r=303":
					case "seeother":
						options.DefaultResponseStatusCode = HttpStatusCode.SeeOther;
						break;

					case "r=307":
					case "temp":
						options.DefaultResponseStatusCode = HttpStatusCode.TemporaryRedirect;
						break;
				}
			}

			return options;
		}
	}
}
