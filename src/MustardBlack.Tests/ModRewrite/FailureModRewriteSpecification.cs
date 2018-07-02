using FluentAssertions;

namespace MustardBlack.Tests.ModRewrite
{
	abstract class FailureModRewriteSpecification : ModRewriteSpecification
	{
		
		[Then]
		public void ShouldNotMatch()
		{
			this.handledResponse.Should().BeNull();
		}
	}
}
