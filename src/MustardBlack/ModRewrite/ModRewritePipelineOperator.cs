using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using MustardBlack.Hosting;
using MustardBlack.Pipeline;
using MustardBlack.Results;
using NanoIoC;
using Serilog;

namespace MustardBlack.ModRewrite
{
    public sealed class ModRewritePipelineOperator : IPreResultPipelineOperator
    {
        static readonly ILogger log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);
        readonly IContainer container;
        IEnumerable<Ruleset> rulesets;

        public ModRewritePipelineOperator(IContainer container, IFileSystem fileSystem)
        {
            this.container = container;
            fileSystem.Read("~/.htaccess", reader => this.rulesets = Parser.Parse(reader.ReadToEnd()));
        }

        public Task<PipelineContinuation> Operate(PipelineContext context)
        {
            var url = context.Request.Url.ToString();

            log.Debug("ModRewrite: Finding Rules: Evaluating {url} condition(s)", url);

            var result = Engine.Execute(rulesets, context.Request);

            if (result != null)
            {
                LogSuccess(url, result);

                context.Result = result;

                var resultExecutorType = typeof(IResultExecutor<>).MakeGenericType(context.Result.GetType());
                var resultExecutor = this.container.Resolve(resultExecutorType) as IResultExecutor;

                resultExecutor?.Execute(context, context.Result);

                return Task.FromResult(PipelineContinuation.End);
            }

            log.Debug("ModRewrite: No Rules Found for {url}", url);
            return Task.FromResult(PipelineContinuation.Continue);
        }

        static void LogSuccess(string url, IResult result)
        {
            switch (result)
            {
                case EmptyResult emptyResult:
                    log.Debug("ModRewrite: Rules Found: Empty Result {originalUrl} with statusCode {statusCode}", url, emptyResult.StatusCode);
                    break;
                case RedirectResult redirectResult:
                    log.Debug("ModRewrite: Rules Found: Redirected {originalUrl} to {rewrittenUrl} with statusCode {statusCode}", url, redirectResult.Location, redirectResult.StatusCode);
                    break;
            }
        }
    }
}