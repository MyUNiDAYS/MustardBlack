using System;
using FluentAssertions;
using FluentAssertions.Extensions;
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
			this.subject.CachePolicy.Should().Be(CachePolicy.Private);
		}
		
		[Then]
		public void ShouldHaveNoRevalidateCacheRevalidation()
		{
			this.subject.CacheRevalidation.Should().Be(HttpCacheRevalidation.None);
		}
		
		[Then]
		public void ShouldHaveANowLastModified()
		{
			this.subject.LastModified.Should().BeCloseTo(DateTime.UtcNow, 1.Seconds());
		}

		[Then]
		public void ExpiresShouldBeNull()
		{
			this.subject.Expires.Should().BeNull();
		}

		[Then]
		public void MaxAgeShouldBeNull()
		{
			this.subject.MaxAge.Should().BeNull();
		}
		
		sealed class TestResult : Result
		{	
		}
	}
}
