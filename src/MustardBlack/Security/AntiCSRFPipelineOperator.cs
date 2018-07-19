using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using MustardBlack.Pipeline;
using MustardBlack.Results;
using Serilog;

namespace MustardBlack.Security
{
	public sealed class AntiCSRFPipelineOperator : IPreResultPipelineOperator
	{
		static readonly ILogger log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

		public Task<PipelineContinuation> Operate(PipelineContext context)
		{
			var originHeader = context.Request.Headers.Origin();
			if (!string.IsNullOrEmpty(originHeader) && !originHeader.EndsWith(context.Request.Url.Domain()))
			{
				log.Debug("Rejecting request, failed CSRF check");

				context.Result = new PlainTextResult("Bad origin header", HttpStatusCode.BadRequest);
				return Task.FromResult(PipelineContinuation.SkipToPostHandler);
			}

			return Task.FromResult(PipelineContinuation.Continue);
		}
	}
}