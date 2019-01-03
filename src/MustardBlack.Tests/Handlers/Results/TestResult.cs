using System;
using System.Collections.Generic;
using System.Net;
using MustardBlack.Caching;
using MustardBlack.Results;

namespace MustardBlack.Tests.Handlers.Results
{
	public class TestResult : IResult
	{
		public HttpStatusCode StatusCode { get; set; }
		public CachePolicy CachePolicy { get; set; }
		public HttpCacheRevalidation CacheRevalidation { get; set; }
		public DateTime LastModified { get; set; }
		public DateTime? Expires { get; set; }
		public TimeSpan? MaxAge { get; set; }
		public IDictionary<string, object> TempData { get; }
		public IList<LinkHeader> Links { get; }
	}
}