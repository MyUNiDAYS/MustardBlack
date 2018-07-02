using System.Collections.Generic;
using MustardBlack.Applications;
using MustardBlack.Hosting;
using MustardBlack.Results;

namespace MustardBlack.Pipeline
{
	/// <summary>
	/// Context for each executing pipeline
	/// </summary>
	public sealed class PipelineContext
	{
        public IApplication Application { get; set; }

		/// <summary>
		/// The Request for this pipeline
		/// </summary>
		public IRequest Request { get; }

		/// <summary>
		/// The Response for this pipeline
		/// </summary>
		public IResponse Response { get; }

		/// <summary>
		/// The result of the pipeline execution.
		/// Setting this before Handler execution prevents the Handler from executing
		/// </summary>
		public IResult Result { get; set; }
		
		/// <summary>
		/// Collection of data related to this pipeline
		/// </summary>
		public IDictionary<string, object> Items { get; }

		/// <summary>
		/// Creates a new PipelineContext
		/// </summary>
		/// <param name="request"></param>
		/// <param name="response"></param>
		public PipelineContext(IRequest request, IResponse response)
		{
			this.Items = new Dictionary<string, object>();

			this.Request = request;
			this.Items["Request"] = request;

			this.Response = response;
		}
	}
}