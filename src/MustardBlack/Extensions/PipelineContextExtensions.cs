using MustardBlack.Pipeline;
using MustardBlack.Routing;

// ReSharper disable once CheckNamespace
namespace MustardBlack
{
	/// <summary>
	/// Extension methods for the PipelineContext
	/// </summary>
	public static class PipelineContextExtensions
	{
		/// <summary>
		/// Gets the RouteData from the context
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public static RouteData RouteData(this PipelineContext context)
		{
			if (context.Items.ContainsKey("RouteData"))
				return context.Items["RouteData"] as RouteData;
			return null;
		}

		/// <summary>
		/// Sets the RouteData in the context
		/// </summary>
		/// <param name="context"></param>
		/// <param name="data"></param>
		public static void RouteData(this PipelineContext context, RouteData data)
		{
			context.Items["RouteData"] = data;
		}
		
		/// <summary>
		/// Gets the AreaName for the current Area
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public static string AreaName(this PipelineContext context)
		{
			return (string)context.RouteData().Values["AreaName"];
		}
	}
}
