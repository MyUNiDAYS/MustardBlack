using MustardBlack.Results;
using System.Threading.Tasks;

namespace MustardBlack.Pipeline
{
    /// <summary>
    /// Removes trailing slashes from paths by issuding a 301 redirect to the same path sans-slash
    /// </summary>
	public sealed class StripTrailingSlashPipelineOperator : IPreHandlerExecutionPipelineOperator
	{
        public Task<PipelineContinuation> Operate(PipelineContext context)
        {
            if (context.Request.Url.Path.Length > 1 && request.Url.Path.EndsWith('/'))
            {
                var newUrl = new Url(request.Url);
                newUrl.Path = newUrl.Path.TrimEnd('/');

                context.Result = RedirectResult.Moved(newUrl);
                return PipelineContinuation.SkipToPostHandler;
            }

            return PipelineContinuation.Continue;
        }
	}
}
