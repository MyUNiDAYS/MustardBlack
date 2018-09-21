using System;
using System.Globalization;
using System.Web;
using MustardBlack.ViewEngines.Razor.Internal;

namespace MustardBlack.ViewEngines.Razor
{
	public abstract class RazorViewPage : RazorPage, IView
	{
		public override void SetHelpers(HtmlHelper htmlHelper, UrlHelper urlHelper)
		{
			this.Html = htmlHelper;
			this.Url = urlHelper;
		}
		
		/// <summary>
		/// Gets the Html helper.
		/// </summary>
		public override HtmlHelper Html { get; protected set; }

		/// <summary>
		/// Gets the Url helper.
		/// </summary>
		public override UrlHelper Url { get; protected set; }
		

//		protected virtual string BuildAttribute(Tuple<string, int> prefix, Tuple<string, int> suffix, params AttributeValue[] values)
//		{
//			var writtenAttribute = false;
//			var attributeBuilder = new StringBuilder();
//			
//			attributeBuilder.Append(prefix.Item1);
//			
//            foreach (var value in values)
//			{
//				if (ShouldWriteValue(value.Value.Item1))
//				{
//					var stringValue = GetStringValue(value);
//					var valuePrefix = value.Prefix.Item1;
//					
//					if (!(value.Value.Item1 is IHtmlString))
//						stringValue = this.HtmlEncode(stringValue, true);
//
//					if (!string.IsNullOrEmpty(valuePrefix))
//						attributeBuilder.Append(valuePrefix);
//
//					attributeBuilder.Append(stringValue);
//					writtenAttribute = true;
//				}
//			}
//
//			attributeBuilder.Append(suffix.Item1);
//
//			// remove empty attributes
//			var renderAttribute = writtenAttribute || values.Length == 0;
//
//			if (renderAttribute)
//				return attributeBuilder.ToString();
//
//			return string.Empty;
//		}
//
//		protected static string GetStringValue(AttributeValue value)
//		{
//			if (value.IsLiteral)
//				return (string)value.Value.Item1;
//
//			if (value.Value.Item1 is IHtmlString)
//				return ((IHtmlString)value.Value.Item1).ToHtmlString();
//
//			return value.Value.Item1.ToString();
//		}
		
		/// <summary>
		/// Html encodes an object if required
		/// </summary>
		/// <param name="value">Object to potentially encode</param>
		/// <param name="insideAttribute"></param>
		/// <returns>String representation, encoded if necessary</returns>
		protected virtual string HtmlEncode(object value, bool insideAttribute)
		{
			if (value == null)
				return null;

			var htmlString = value as IHtmlString;
			if (htmlString != null)
				return htmlString.ToHtmlString();
			
			var s = Convert.ToString(value, CultureInfo.CurrentUICulture);
			return insideAttribute ? HttpUtility.HtmlAttributeEncode(s) : HttpUtility.HtmlEncode(s);
		}
	}
}
