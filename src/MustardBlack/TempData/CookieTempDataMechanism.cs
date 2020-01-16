using System;
using System.Collections.Generic;
using MustardBlack.Hosting;
using MustardBlack.Pipeline;
using Newtonsoft.Json;

namespace MustardBlack.TempData
{
	public sealed class CookieTempDataMechanism : ITempDataMechanism
	{
		public static Func<IRequest, string> GetCookieDomain = request => request.Url.Domain();

		public void SetTempData(PipelineContext context, IDictionary<string, object> tempData)
		{
			if (tempData.Keys.Count <= 0)
				return;

			var serializeObject = JsonConvert.SerializeObject(tempData);

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