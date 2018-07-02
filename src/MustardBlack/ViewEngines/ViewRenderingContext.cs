using System.Collections.Generic;
using MustardBlack.Hosting;

namespace MustardBlack.ViewEngines
{
	public sealed class ViewRenderingContext
	{
		public Url RequestUrl { get; set; } 
		public IRequestState RequestState { get; set; } 
		public IDictionary<string, object> ContextItems { get; set; }
	}
}