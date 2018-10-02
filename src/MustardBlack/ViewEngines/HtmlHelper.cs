using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
using MustardBlack.Hosting;
using MustardBlack.Results;

namespace MustardBlack.ViewEngines
{
	public class HtmlHelper : IHtmlHelper
	{
	    public TextWriter Writer { get; }
	    public Url RequestUrl { get; }
		public IRequestState RequestState { get; }
		public ViewResult ViewResult { get; }

		public HtmlEncoder Encoder { get; set; }

		public IDictionary<string, object> ContextItems { get; }

		public HtmlHelper(ViewResult viewResult, Url requestUrl, HtmlEncoder encoder, IRequestState requestState, IDictionary<string, object> contextItems, TextWriter writer)
		{
		    this.Writer = writer;
		    this.RequestUrl = requestUrl;
		    this.Encoder = encoder;
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

	    /// <inheritdoc />
	    public IHtmlContent Raw(object value)
		{
			return new HtmlString(value.ToString());
		}
	}
}
