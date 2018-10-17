

namespace MustardBlack.Tests.ModRewrite
{
	public abstract class FailureModRewriteSpecification : ModRewriteSpecification
	{
		
		[Then]
		public void ShouldNotMatch()
		{
			this.handledResponse.ShouldBeNull();
		}
	}
}
