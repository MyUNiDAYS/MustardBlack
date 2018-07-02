using System;
using System.Collections.Generic;
using System.Net;
using MustardBlack.Caching;

namespace MustardBlack.Results
{
	public interface IResult
	{
		HttpStatusCode StatusCode { get; set; }
		CachePolicy CachePolicy { get; set; }
        HttpCacheRevalidation CacheRevalidation { get; set; }

		DateTime LastModified { get; set; }
		DateTime? Expires { get; set; }
		TimeSpan? MaxAge { get; set; }
		
		IDictionary<string, object> TempData { get; }
		IList<LinkHeader> Links { get; }
	}
}