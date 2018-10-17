using System;
using MustardBlack.Caching;
using MustardBlack.Results;

namespace MustardBlack.Tests.Handlers.Results.ResultSpecs
{
	public class DefaultCachePolicy : Specification
	{
		TestResult subject;
		
		protected override void When()
		{
			this.subject = new TestResult();
		}

		[Then]
		public void ShouldHavePrivateCachePolicy()
		{
			this.subject.CachePolicy.ShouldEqual(CachePolicy.Private);
		}
		
		[Then]
		public void ShouldHaveNoRevalidateCacheRevalidation()
		{
			this.subject.CacheRevalidation.ShouldEqual(HttpCacheRevalidation.None);
		}
		
		[Then]
		public void ShouldHaveANowLastModified()
		{
			this.subject.LastModified.ShouldBeInRange(DateTime.UtcNow, TimeSpan.FromSeconds(1));
		}

		[Then]
		public void ExpiresShouldBeNull()
		{
			this.subject.Expires.ShouldBeNull();
		}

		[Then]
		public void MaxAgeShouldBeNull()
		{
			this.subject.MaxAge.ShouldBeNull();
		}
		
		sealed class TestResult : Result
		{	
		}
	}
}
