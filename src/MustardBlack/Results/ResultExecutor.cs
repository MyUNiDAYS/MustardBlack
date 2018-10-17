using System;
using System.Text;
using MustardBlack.Pipeline;

namespace MustardBlack.Results
{
	public abstract class ResultExecutor<TResult> : IResultExecutor<TResult> where TResult : IResult
	{
		public abstract void Execute(PipelineContext context, TResult result);

		public void Execute(PipelineContext context, object result)
		{
			if(result == null)
				throw new ArgumentException("Result is cannot be null");

			if (result.GetType() != typeof(TResult))
				throw new ArgumentException("Result is not of type `" + typeof(TResult) + "`", nameof(result));
		
			this.Execute(context, (TResult)result);
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
