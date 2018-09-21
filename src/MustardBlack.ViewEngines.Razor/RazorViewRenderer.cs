using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using MustardBlack.Results;
using MustardBlack.ViewEngines.Razor.Internal;

namespace MustardBlack.ViewEngines.Razor
{
	/// <summary>
	/// TODO: This needs to be transient
	/// </summary>
	public class RazorViewRenderer : ViewRendererBase
	{
		readonly IViewResolver viewResolver;
		readonly IViewBufferScope bufferScope;
		private readonly HtmlEncoder htmlEncoder;
		IRazorPage razorPage;

		public RazorViewRenderer(IViewResolver viewResolver, IViewBufferScope viewBufferScope, HtmlEncoder htmlEncoder)
		{
			this.viewResolver = viewResolver;
			this.bufferScope = viewBufferScope;
			this.htmlEncoder = htmlEncoder;
		}

		public override bool CanRender(Type viewType)
		{
			return viewType.IsOrDerivesFrom(typeof(RazorPage));
		}

		public override async Task<StringBuilder> Render(ViewResult viewResult, ViewRenderingContext context)
		{
			var viewContext = new RazorViewContext
			{
				Writer = new StringWriter()
			};

			this.razorPage = GetViewInstance(viewResult, context);

			var bodyWriter = await RenderPageAsync(razorPage, viewContext);
			await RenderLayoutAsync(viewResult, viewContext, bodyWriter);

			return new StringBuilder(viewContext.Writer.ToString());
		}

		async Task<ViewBufferTextWriter> RenderPageAsync(IRazorPage page, RazorViewContext context)
		{
			var writer = context.Writer as ViewBufferTextWriter;
			if (writer == null)
			{
				// If we get here, this is likely the top-level page (not a partial) - this means
				// that context.Writer is wrapping the output stream. We need to buffer, so create a buffered writer.
				var buffer = new ViewBuffer(this.bufferScope, page.Path, ViewBuffer.ViewPageSize);
				writer = new ViewBufferTextWriter(buffer, context.Writer.Encoding, this.htmlEncoder, context.Writer);
			}
			else
			{
				// This means we're writing something like a partial, where the output needs to be buffered.
				// Create a new buffer, but without the ability to flush.
				var buffer = new ViewBuffer(this.bufferScope, page.Path, ViewBuffer.ViewPageSize);
				writer = new ViewBufferTextWriter(buffer, context.Writer.Encoding);
			}

			// The writer for the body is passed through the ViewContext, allowing things like HtmlHelpers
			// and ViewComponents to reference it.
			var oldWriter = context.Writer;
			//var oldFilePath = context.ExecutingFilePath;

			context.Writer = writer;
			//context.ExecutingFilePath = page.Path;

			try
			{
				page.RazorViewContext = context;
				await page.ExecuteAsync();
				return writer;
			}
			finally
			{
				context.Writer = oldWriter;
				//context.ExecutingFilePath = oldFilePath;
			}
		}
		
		async Task RenderLayoutAsync(ViewResult viewResult, RazorViewContext context, ViewBufferTextWriter bodyWriter)
		{
			// A layout page can specify another layout page. We'll need to continue
			// looking for layout pages until they're no longer specified.
			var previousPage = this.razorPage;
			var renderedLayouts = new List<IRazorPage>();

			// This loop will execute Layout pages from the inside to the outside. With each
			// iteration, bodyWriter is replaced with the aggregate of all the "body" content
			// (including the layout page we just rendered).
			while (!string.IsNullOrEmpty(previousPage.Layout))
			{
				if (!bodyWriter.IsBuffering)
				{
					// Once a call to RazorPage.FlushAsync is made, we can no longer render Layout pages - content has
					// already been written to the client and the layout content would be appended rather than surround
					// the body content. Throwing this exception wouldn't return a 500 (since content has already been
					// written), but a diagnostic component should be able to capture it.

					var message = "Resources.FormatLayoutCannotBeRendered(Path, nameof(New.RazorPage.FlushAsync))";
					throw new InvalidOperationException(message);
				}

				var layoutPage = GetLayoutPage(viewResult, context, previousPage.Path, previousPage.Layout);

				if (renderedLayouts.Count > 0 && renderedLayouts.Any(l => string.Equals(l.Path, layoutPage.Path, StringComparison.Ordinal)))
				{
					// If the layout has been previously rendered as part of this view, we're potentially in a layout
					// rendering cycle.
					throw new InvalidOperationException("Resources.FormatLayoutHasCircularReference(previousPage.Path, layoutPage.Path)");
				}

				// Notify the previous page that any writes that are performed on it are part of sections being written
				// in the layout.
				previousPage.IsLayoutBeingRendered = true;
				layoutPage.PreviousSectionWriters = previousPage.SectionWriters;
				layoutPage.BodyContent = bodyWriter.Buffer;
				bodyWriter = await RenderPageAsync(layoutPage, context);

				renderedLayouts.Add(layoutPage);
				previousPage = layoutPage;
			}

			// Now we've reached and rendered the outer-most layout page. Nothing left to execute.

			// Ensure all defined sections were rendered or RenderBody was invoked for page without defined sections.
			foreach (var layoutPage in renderedLayouts)
				layoutPage.EnsureRenderedBodyOrSections();

			if (bodyWriter.IsBuffering)
			{
				// If IsBuffering - then we've got a bunch of content in the view buffer. How to best deal with it
				// really depends on whether or not we're writing directly to the output or if we're writing to
				// another buffer.
				var viewBufferTextWriter = context.Writer as ViewBufferTextWriter;
				if (viewBufferTextWriter == null || !viewBufferTextWriter.IsBuffering)
				{
					// This means we're writing to a 'real' writer, probably to the actual output stream.
					// We're using PagedBufferedTextWriter here to 'smooth' synchronous writes of IHtmlContent values.
					using (var writer = this.bufferScope.CreateWriter(context.Writer))
					{
						await bodyWriter.Buffer.WriteToAsync(writer, this.htmlEncoder);
					}
				}
				else
				{
					// This means we're writing to another buffer. Use MoveTo to combine them.
					bodyWriter.Buffer.MoveTo(viewBufferTextWriter.Buffer);
				}
			}
		}

		IRazorPage GetLayoutPage(ViewResult viewResult, RazorViewContext context, string executingFilePath, string layoutPath)
		{
			Type layoutType;
			try
			{
				layoutType = this.viewResolver.Resolve(layoutPath);
			}
			catch (Exception ex)
			{
				throw new Exception($"Unable to resolve layout for view: {layoutPath}", ex);
			}

			var layoutResult = new ViewResult(layoutType, viewResult.AreaName, viewResult.ViewData, viewResult.StatusCode);
			return  this.GetViewInstance(layoutResult, null);
		}

		IRazorPage GetViewInstance(ViewResult viewResult, ViewRenderingContext context)
		{
			var view = Activator.CreateInstance(viewResult.ViewType) as IRazorPage;
			this.HydrateView(viewResult, view, context);
			return view;
		}
	}
}