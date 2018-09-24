using System.Collections.Generic;
using System.IO;
using MustardBlack.Hosting;

namespace MustardBlack.ViewEngines
{
	public sealed class ViewRenderingContext
	{
		public Url RequestUrl { get; set; } 
		public IRequestState RequestState { get; set; } 
		public IDictionary<string, object> ContextItems { get; set; }
		/// <summary>
		/// Gets or sets the <see cref="TextWriter"/> used to write the output.
		/// </summary>
		public TextWriter Writer { get; set; }
	}
}