using System;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Html;
using MustardBlack.Results;
using NanoIoC;
using Serilog;

namespace MustardBlack.ViewEngines
{
    public static class HtmlHelperExtensions
    {
        static readonly ILogger log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Renders a View
        /// </summary>
        /// <param name="view"></param>
        /// <param name="viewPath">The virtual path to the view</param>
        /// <param name="viewData">The data for the view</param>
        // TODO: Make async version
        public static IHtmlContent RenderView(this IView view, string viewPath, object viewData = null)
        {
            if (string.IsNullOrEmpty(viewPath))
                throw new ArgumentException("viewPath");

            if (!viewPath.EndsWith(".cshtml"))
                viewPath += ".cshtml";

            try
            {
                var viewType = view.Container.Resolve<IViewResolver>().Resolve(viewPath);
                var viewEngine = view.Container.Resolve<IViewRendererFinder>().FindViewRenderer(viewType);
                var viewResult = new ViewResult(viewType, view.ViewResult.AreaName, viewData);

                var stringBuilder = new StringBuilder();
                var renderingContext = new ViewRenderingContext
                {
                    Writer = new StringWriter(stringBuilder)
                };

                var renderTask = viewEngine.Render(viewResult, view.PipelineContext, renderingContext);
                renderTask.GetAwaiter().GetResult();

                // TODO: is this still the best approach?
                return new HtmlString(stringBuilder.ToString());
            }
            catch (Exception e)
            {
                log.Error(e, "Failed to render view `{ViewPath}`", viewPath);
                return null;
            }
        }
    }
}