using System;
using System.Collections.Generic;
using System.Net;
using MustardBlack.Caching;

namespace MustardBlack.Results
{
	public abstract class Result : IResult
	{
		public HttpStatusCode StatusCode { get; set; }
		public CachePolicy CachePolicy { get; set; }
	    public HttpCacheRevalidation CacheRevalidation { get; set; }
        public DateTime LastModified { get; set; }
		public DateTime? Expires { get; set; }
		public TimeSpan? MaxAge { get; set; }
		
		public IDictionary<string, object> TempData { get; }
		public IList<LinkHeader> Links { get; set; }

		protected Result()
		{
			this.CachePolicy = CachePolicy.Private;
            this.CacheRevalidation = HttpCacheRevalidation.None;
			this.LastModified = DateTime.UtcNow;
			
			this.TempData = new Dictionary<string, object>();
			this.Links = new List<LinkHeader>();
		}

		public void SetTempData(string key, object value)
		{
			this.TempData[key] = value;
		}
	}
}