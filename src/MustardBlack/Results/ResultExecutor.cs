using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using MustardBlack.Pipeline;
using Newtonsoft.Json;
using Serilog;

namespace MustardBlack.Results
{
	public abstract class ResultExecutor<TResult> : IResultExecutor<TResult> where TResult : IResult
	{
		static readonly ILogger log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

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

		protected static void SetTempData(PipelineContext context, IDictionary<string, object> tempData)
		{
			if (tempData.Keys.Count <= 0)
				return;

			var serializeObject = JsonConvert.SerializeObject(tempData);

			// Temporary logging to see if we're outputting commas in cookies, we shouldnt be as it can b0rk things.
			if (serializeObject != null && serializeObject.Contains(","))
				log.Warning("Temp data contains a comma `{0}`", serializeObject);

			var cookie = new ResponseCookie(
				"temp",
				value: serializeObject,
                secure: true,
				httpOnly: false,
				domain: context.Request.Url.Domain()
			);
			
			context.Response.Cookies.Set(cookie);
		}
	}
}
