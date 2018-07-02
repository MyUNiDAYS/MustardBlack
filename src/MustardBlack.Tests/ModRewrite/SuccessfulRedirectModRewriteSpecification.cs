using System.Net;
using FluentAssertions;
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

			redirectResult.Location.ToString().Should().Be(this.ExpectedRedirectUrl);
			redirectResult.StatusCode.Should().Be(this.ExpectedStatusCode);
		}
	}
}
