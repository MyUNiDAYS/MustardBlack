using System.Collections.Generic;
using System.Text.Encodings.Web;
using MustardBlack.Hosting;
using MustardBlack.Results;

namespace MustardBlack.ViewEngines
{
	public class HtmlHelper : IHtmlHelper
	{
		public Url RequestUrl { get; }
		public IRequestState RequestState { get; }
		public ViewResult ViewResult { get; }

		public HtmlEncoder Encoder { get; set; }

		public IDictionary<string, object> ContextItems { get; }

		public HtmlHelper(ViewResult viewResult, Url requestUrl, IRequestState requestState, IDictionary<string, object> contextItems)
		{
			this.RequestUrl = requestUrl;
			this.ViewResult = viewResult;
			this.RequestState = requestState;
			this.ContextItems = contextItems;
		}
		
		/// <summary>
		/// HTML Encodes the given string
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public string Encode(string text)
		{
			return this.Encoder.Encode(text);
		}
	}
}
