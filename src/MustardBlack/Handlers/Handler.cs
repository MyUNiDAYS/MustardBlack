using System.Collections.Generic;
using MustardBlack.Hosting;
using MustardBlack.Pipeline;
using MustardBlack.Results;
using MustardBlack.ViewEngines;
using NanoIoC;

namespace MustardBlack.Handlers
{
	/// <summary>
	/// Abstract request handler
	/// </summary>
	public abstract class Handler : IHandler
	{
		// TODO: log base here if possible

		public PipelineContext Context { get; set; }
		
		/// <summary>
		/// Creates a new ViewResult
		/// </summary>
		/// <param name="name">The name of the view</param>
		/// <param name="resource">The data associated with the view</param>
		/// <returns></returns>
		protected ViewResult View(string name, object resource = null)
		{
			var viewResolver = Container.Global.Resolve<IViewResolver>();

			if (!name.StartsWith("~"))
			{
				var requestingNamespace = this.GetType().Namespace.Substring(this.GetType().Assembly.GetName().Name.Length + 1);
				name = "~/" + requestingNamespace.Replace('.', '/') + '/' + name.Substring(name.LastIndexOf('/') + 1);
			}

			if (!name.EndsWith(".cshtml"))
				name += ".cshtml";

			var viewType = viewResolver.Resolve(name);
			var viewResult = new ViewResult(viewType, this.Context.AreaName(), resource);
			
			return viewResult;
		}

		/// <summary>
		/// Creates a new File result
		/// </summary>
		/// <param name="data">The file data</param>
		/// <param name="contentType">The content-type of the file</param>
		/// <returns></returns>
		protected FileContentResult File(string contentType, byte[] data)
		{
			return new FileContentResult(contentType, data);
		}
		
		public virtual IEnumerable<HrefLang> GetAlternateLangs(IRequest request)
		{
			return null;
		}
	}
}
