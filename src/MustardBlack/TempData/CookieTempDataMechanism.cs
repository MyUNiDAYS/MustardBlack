using System;
using System.Collections.Generic;
using System.Reflection;
using MustardBlack.Hosting;
using MustardBlack.Pipeline;
using Newtonsoft.Json;
using Serilog;

namespace MustardBlack.TempData
{
	public sealed class CookieTempDataMechanism : ITempDataMechanism
	{
		static readonly ILogger log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

		public static Func<IRequest, string> GetCookieDomain = request => request.Url.Domain();

		public void SetTempData(PipelineContext context, IDictionary<string, object> tempData)
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
				domain: GetCookieDomain(context.Request)
			);

			context.Response.Cookies.Set(cookie);
		}
	}
}