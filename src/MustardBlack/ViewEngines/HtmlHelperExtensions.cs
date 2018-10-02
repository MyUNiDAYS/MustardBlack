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
        /// <param name="html"></param>
        /// <param name="viewPath">The virtual path to the view</param>
        /// <param name="viewData">The data for the view</param>
        public static IHtmlContent RenderView(this HtmlHelper html, string viewPath, object viewData = null)
        {
            if (string.IsNullOrEmpty(viewPath))
                throw new ArgumentException("viewPath");

            if (!viewPath.EndsWith(".cshtml"))
                viewPath += ".cshtml";

            try
            {
                var viewType = Container.Global.Resolve<IViewResolver>().Resolve(viewPath);
                var viewEngine = Container.Global.Resolve<IViewRendererFinder>().FindViewRenderer(viewType);
                var viewResult = new ViewResult(viewType, html.ViewResult.AreaName, viewData);

                var stringBuilder = new StringBuilder();
                var renderingContext = new ViewRenderingContext
                {
                    RequestUrl = html.RequestUrl,
                    RequestState = html.RequestState,
                    ContextItems = html.ContextItems,
                    Writer = new StringWriter(stringBuilder)
                };

                var renderTask = viewEngine.Render(viewResult, renderingContext);
                renderTask.GetAwaiter().GetResult();

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