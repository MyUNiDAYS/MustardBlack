using System;
using Microsoft.AspNetCore.Html;
using MustardBlack.Pipeline;
using MustardBlack.Results;
using MustardBlack.ViewEngines.Razor.Internal;
using NanoIoC;
using HtmlString = Microsoft.AspNetCore.Html.HtmlString;

namespace MustardBlack.ViewEngines.Razor
{
	public abstract class RazorViewPage : RazorPage
	{
		public override ViewResult ViewResult { get; set; }
		public override PipelineContext PipelineContext { get; set; }
		public override IContainer Container { get; set; }
		
		/// <summary>
		/// Html encodes an object if required
		/// </summary>
		/// <param name="value">Object to potentially encode</param>
		/// <returns>String representation, encoded if necessary</returns>
		public virtual IHtmlContent Raw(string value)
		{
			if (value == null)
				return null;
			
			return new HtmlString(value);
		}

		[Obsolete("Do not use, use `this` instead.")]
		protected RazorViewPage Html => this;

		[Obsolete("Do not use, use `this` instead.")]
		protected RazorViewPage Url => this;
	}
}
