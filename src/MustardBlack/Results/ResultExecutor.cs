using System;
using System.Text;
using System.Threading.Tasks;
using MustardBlack.Pipeline;

namespace MustardBlack.Results
{
	public abstract class ResultExecutor<TResult> : IResultExecutor<TResult> where TResult : IResult
	{
		public abstract Task Execute(PipelineContext context, TResult result);

		public Task Execute(PipelineContext context, object result)
		{
			if(result == null)
				throw new ArgumentException("Result is cannot be null");

			if (!result.GetType().IsOrDerivesFrom<TResult>())
				throw new ArgumentException("Result is not/does not derive from type `" + typeof(TResult) + "`", nameof(result));
		
			return this.Execute(context, (TResult)result);
		}

		protected static void SetLinkHeaders(PipelineContext context, IResult result)
		{
			var builder = new StringBuilder();
			
			for (var i  = 0; i < result.Links.Count; i++)
			{
				builder.Append('<').Append(result.Links[i].Href).Append(">; rel=\"").Append(result.Links[i].Rel).Append('"');

				if (!string.IsNullOrEmpty(result.Links[i].Media))
					builder.Append("; media=\"").Append(result.Links[i].Media).Append('"');

				if (!string.IsNullOrEmpty(result.Links[i].HrefLang))
					builder.Append("; hreflang=\"").Append(result.Links[i].HrefLang).Append('"');

				if (i < result.Links.Count - 1)
					builder.Append(", ");
			}

			context.Response.Headers.Add("Link", builder.ToString());
		}
	}
}
