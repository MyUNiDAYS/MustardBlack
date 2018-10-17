using System.Net;

using MustardBlack.Results;

namespace MustardBlack.Tests.ModRewrite
{
	public abstract class SuccessfulRedirectModRewriteSpecification : ModRewriteSpecification
	{
		protected abstract string ExpectedRedirectUrl { get; }
		protected abstract HttpStatusCode ExpectedStatusCode { get; }
		
		[Then]
		public void ShouldRedirectProperly()
		{
			var redirectResult = this.handledResponse as RedirectResult;

			redirectResult.Location.ToString().ShouldEqual(this.ExpectedRedirectUrl);
			redirectResult.StatusCode.ShouldEqual(this.ExpectedStatusCode);
		}
	}
}
