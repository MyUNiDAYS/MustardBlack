using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using MustardBlack;
using MustardBlack.Results;
using MustardBlack.ViewEngines.Razor.Internal;
using NanoIoC;

namespace MustardBlack.ViewEngines.Razor
{
	public class RazorViewRenderer : ViewRendererBase
	{
		readonly IViewResolver viewResolver;
		readonly IContainer container;

		public RazorViewRenderer(IViewResolver viewResolver, IContainer container, HtmlEncoder htmlEncoder) : base(htmlEncoder)
		{
			this.viewResolver = viewResolver;
			this.container = container;
		}

		public override bool CanRender(Type viewType)
		{
			return viewType.IsOrDerivesFrom(typeof(RazorPage));
		}

		public override async Task Render(ViewResult viewResult, ViewRenderingContext viewRenderingContext)
		{
			var razorPage = this.GetViewInstance(viewResult, viewRenderingContext);

			var viewBufferScope = this.container.Resolve<IViewBufferScope>();

			var bodyWriter = await this.RenderPageAsync(viewBufferScope, razorPage, viewRenderingContext);
			await this.RenderLayoutAsync(viewBufferScope, razorPage, viewResult, viewRenderingContext, bodyWriter);
		}

		async Task<ViewBufferTextWriter> RenderPageAsync(IViewBufferScope bufferScope, IRazorPage page, ViewRenderingContext viewRenderingContext)
		{
			var writer = viewRenderingContext.Writer as ViewBufferTextWriter;
			if (writer == null)
			{
				// If we get here, this is likely the top-level page (not a partial) - this means
				// that context.Writer is wrapping the output stream. We need to buffer, so create a buffered writer.
				var buffer = new ViewBuffer(bufferScope, page.GetType().AssemblyQualifiedName, ViewBuffer.ViewPageSize);
				writer = new ViewBufferTextWriter(buffer, viewRenderingContext.Writer.Encoding, this.htmlEncoder, viewRenderingContext.Writer);
			}
			else
			{
				// This means we're writing something like a partial, where the output needs to be buffered.
				// Create a new buffer, but without the ability to flush.
				var buffer = new ViewBuffer(bufferScope, page.GetType().AssemblyQualifiedName, ViewBuffer.ViewPageSize);
				writer = new ViewBufferTextWriter(buffer, viewRenderingContext.Writer.Encoding);
			}

			// The writer for the body is passed through the ViewContext, allowing things like HtmlHelpers
			// and ViewComponents to reference it.
			var oldWriter = viewRenderingContext.Writer;
			//var oldFilePath = context.ExecutingFilePath;

			viewRenderingContext.Writer = writer;
			//context.ExecutingFilePath = page.Path;

			try
			{
				page.RenderingContext = viewRenderingContext;
				await page.ExecuteAsync();
				return writer;
			}
			finally
			{
				viewRenderingContext.Writer = oldWriter;
				//context.ExecutingFilePath = oldFilePath;
			}
		}

		async Task RenderLayoutAsync(IViewBufferScope bufferScope, IRazorPage previousPage, ViewResult viewResult, ViewRenderingContext viewRenderingContext, ViewBufferTextWriter bodyWriter)
		{
			// A layout page can specify another layout page. We'll need to continue
			// looking for layout pages until they're no longer specified.
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

				var layoutPage = GetLayoutPage(viewResult, viewRenderingContext, previousPage.Layout);

				if (renderedLayouts.Count > 0 && renderedLayouts.Any(l => l.GetType() == layoutPage.GetType()))
				{
					// If the layout has been previously rendered as part of this view, we're potentially in a layout
					// rendering cycle.
					throw new InvalidOperationException("Layout has circular reference. Previous Page: `" + previousPage.GetType() + "`, Current Layout: `" + layoutPage.GetType() + "`");
				}

				// Notify the previous page that any writes that are performed on it are part of sections being written
				// in the layout.
				previousPage.IsLayoutBeingRendered = true;
				layoutPage.PreviousSectionWriters = previousPage.SectionWriters;
				layoutPage.BodyContent = bodyWriter.Buffer;
				bodyWriter = await this.RenderPageAsync(bufferScope, layoutPage, viewRenderingContext);

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
				var viewBufferTextWriter = viewRenderingContext.Writer as ViewBufferTextWriter;
				if (viewBufferTextWriter == null || !viewBufferTextWriter.IsBuffering)
				{
					// This means we're writing to a 'real' writer, probably to the actual output stream.
					// We're using PagedBufferedTextWriter here to 'smooth' synchronous writes of IHtmlContent values.
					using (var writer = bufferScope.CreateWriter(viewRenderingContext.Writer))
						await bodyWriter.Buffer.WriteToAsync(writer, this.htmlEncoder);
				}
				else
				{
					// This means we're writing to another buffer. Use MoveTo to combine them.
					bodyWriter.Buffer.MoveTo(viewBufferTextWriter.Buffer);
				}
			}
		}

		IRazorPage GetLayoutPage(ViewResult viewResult, ViewRenderingContext viewRenderingContext, string layoutPath)
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
			return this.GetViewInstance(layoutResult, viewRenderingContext);
		}

		IRazorPage GetViewInstance(ViewResult viewResult, ViewRenderingContext context)
		{
			var view = Activator.CreateInstance(viewResult.ViewType) as IRazorPage;
			this.HydrateView(viewResult, view, context);
			return view;
		}
	}
}