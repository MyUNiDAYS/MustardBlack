using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;

namespace MustardBlack.ViewEngines.Razor
{
	public abstract class RazorViewPage : IView
	{
		// TODO: temporarilly public, until EUTP-402 merges removing Wrapper.cs
		public readonly StringBuilder contents;
		string childBody;
		IDictionary<string, string> childSections;
		
		void IView.SetHelpers(HtmlHelper htmlHelper, UrlHelper urlHelper)
		{
			this.Html = htmlHelper;
			this.Url = urlHelper;
		}
		
		/// <summary>
		/// Gets the Html helper.
		/// </summary>
		public HtmlHelper Html { get; protected set; }

		/// <summary>
		/// Gets the Url helper.
		/// </summary>
		public UrlHelper Url { get; protected set; }

		/// <summary>
		/// Gets the body.
		/// </summary>
		public string Body { get; protected set; }

		/// <summary>
		/// Gets or sets the section contents.
		/// </summary>
		/// <value>
		/// The section contents.
		/// </value>
		public IDictionary<string, string> SectionContents { get; set; }

		/// <summary>
		/// Gets or sets the layout.
		/// </summary>
		/// <value>
		/// The layout.
		/// </value>
		public virtual string Layout { get; set; }

		/// <summary>
		/// Gets a value indicating whether this instance has layout.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance has layout; otherwise, <c>false</c>.
		/// </value>
		public bool HasLayout => !string.IsNullOrEmpty(this.Layout);

		/// <summary>
		/// Gets or sets the sections.
		/// </summary>
		/// <value>
		/// The sections.
		/// </value>
		public IDictionary<string, Action> Sections { get; set; }

		/// <summary>
		/// Executes the view.
		/// </summary>
		public abstract void Execute();

		protected RazorViewPage()
		{
			this.Sections = new Dictionary<string, Action>();
			this.contents = new StringBuilder();
		}

		/// <summary>
		/// Writes the results of expressions like: "@foo.Bar"
		/// </summary>
		/// <param name="value">The value.</param>
		public virtual void Write(object value)
		{
			this.WriteLiteral(this.HtmlEncode(value, false));
		}

		/// <summary>
		/// Writes literals like markup: "<p>Foo</p>"
		/// </summary>
		/// <param name="value">The value.</param>
		public virtual void WriteLiteral(object value)
		{
			this.contents.Append(value);
		}

		public virtual void WriteAttribute(string name, Tuple<string, int> prefix, Tuple<string, int> suffix, params AttributeValue[] values)
		{
			var attributeValue = this.BuildAttribute(prefix, suffix, values);
			this.WriteLiteral(attributeValue);
		}

		public virtual void WriteAttributeTo(TextWriter writer, string name, Tuple<string, int> prefix, Tuple<string, int> suffix, params AttributeValue[] values)
		{
			var attributeValue = this.BuildAttribute(prefix, suffix, values);
			this.WriteLiteralTo(writer, attributeValue);
		}

		protected virtual string BuildAttribute(Tuple<string, int> prefix, Tuple<string, int> suffix, params AttributeValue[] values)
		{
			var writtenAttribute = false;
			var attributeBuilder = new StringBuilder();
			
			attributeBuilder.Append(prefix.Item1);
			
            foreach (var value in values)
			{
				if (ShouldWriteValue(value.Value.Item1))
				{
					var stringValue = GetStringValue(value);
					var valuePrefix = value.Prefix.Item1;
					
					if (!(value.Value.Item1 is IHtmlString))
						stringValue = this.HtmlEncode(stringValue, true);

					if (!string.IsNullOrEmpty(valuePrefix))
						attributeBuilder.Append(valuePrefix);

					attributeBuilder.Append(stringValue);
					writtenAttribute = true;
				}
			}

			attributeBuilder.Append(suffix.Item1);

			// remove empty attributes
			var renderAttribute = writtenAttribute || values.Length == 0;

			if (renderAttribute)
				return attributeBuilder.ToString();

			return string.Empty;
		}

		protected static string GetStringValue(AttributeValue value)
		{
			if (value.IsLiteral)
				return (string)value.Value.Item1;

			if (value.Value.Item1 is IHtmlString)
				return ((IHtmlString)value.Value.Item1).ToHtmlString();

			return value.Value.Item1.ToString();
		}

		protected static bool ShouldWriteValue(object value)
		{
			if (value == null)
				return false;

			if (value is bool)
				return (bool)value;

			return true;
		}

		/// <summary>
		/// Writes the provided <paramref name="value"/> to the provided <paramref name="writer"/>.
		/// </summary>
		/// <param name="writer">The <see cref="TextWriter"/> that should be written to.</param>
		/// <param name="value">The value that should be written.</param>
		public virtual void WriteTo(TextWriter writer, object value)
		{
			writer.Write(this.HtmlEncode(value, false));
		}

		/// <summary>
		/// Writes the provided <paramref name="value"/>, as a literal, to the provided <paramref name="writer"/>.
		/// </summary>
		/// <param name="writer">The <see cref="TextWriter"/> that should be written to.</param>
		/// <param name="value">The value that should be written as a literal.</param>
		public virtual void WriteLiteralTo(TextWriter writer, object value)
		{
			writer.Write(value);
		}
		protected internal void BeginContext(TextWriter writer, string virtualPath, int startPosition, int length, bool isLiteral)
		{
		}

		protected internal void EndContext(TextWriter writer, string virtualPath, int startPosition, int length, bool isLiteral)
		{
		}

		/// <summary>
		/// Stores sections
		/// </summary>
		/// <param name="sectionName">Name of the section.</param>
		/// <param name="action">The action.</param>
		public virtual void DefineSection(string sectionName, Action action)
		{
			this.Sections.Add(sectionName, action);
		}

		/// <summary>
		/// Renders the section.
		/// </summary>
		/// <param name="sectionName">Name of the section.</param>
		/// <returns></returns>
		public virtual object RenderSection(string sectionName)
		{
			return this.RenderSection(sectionName, false);
		}

		/// <summary>
		/// Renders the section.
		/// </summary>
		/// <param name="sectionName">Name of the section.</param>
		/// <param name="required">if set to <c>true</c> [required].</param>
		public virtual object RenderSection(string sectionName, bool required)
		{
			string sectionContent;

			var exists = this.childSections.TryGetValue(sectionName, out sectionContent);
			if (!exists && required)
			{
				throw new InvalidOperationException("Section name " + sectionName + " not found and is required.");
			}

			this.contents.Append(sectionContent ?? string.Empty);

			return null;
		}

		/// <summary>
		/// Renders the body.
		/// </summary>
		/// <returns></returns>
		public virtual object RenderBody()
		{
			this.contents.Append(this.childBody);

			return null;
		}

		///<summary>
		///Indicates if a section is defined.
		///</summary>
		public virtual bool IsSectionDefined(string sectionName)
		{
			return this.childSections.ContainsKey(sectionName);
		}

		public virtual string ResolveUrl(string url)
		{
			throw new NotImplementedException();
			// Nancy: return this.RenderContext.ParsePath(url);
		}

		/// <summary>
		/// Executes the view.
		/// </summary>
		/// <param name="body">The body.</param>
		/// <param name="sectionContents">The section contents.</param>
		public void ExecuteView(string body, IDictionary<string, string> sectionContents)
		{
			this.childBody = body ?? string.Empty;
			this.childSections = sectionContents ?? new Dictionary<string, string>();

			try
			{
				this.Execute();
			}
			catch (NullReferenceException e)
			{
				throw new ViewRenderException("Unable to render the view.  Most likely the ViewData, or a property on the ViewData, is null", e);
			}

			this.Body = this.contents.ToString();

			this.SectionContents = new Dictionary<string, string>(this.Sections.Count);
			foreach (var section in this.Sections)
			{
				this.contents.Clear();
				try
				{
					section.Value.Invoke();
				}
				catch (NullReferenceException e)
				{
					throw new ViewRenderException($"A null reference was encountered while rendering the section {section.Key}.  Does the section require a model? (maybe it wasn't passed in)", e);
				}
				this.SectionContents.Add(section.Key, this.contents.ToString());
			}
		}

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
