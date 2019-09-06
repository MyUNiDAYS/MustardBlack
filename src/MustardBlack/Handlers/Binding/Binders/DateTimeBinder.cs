using System;
using System.Globalization;
using MustardBlack.Hosting;
using MustardBlack.Routing;

namespace MustardBlack.Handlers.Binding.Binders
{
	public class DateTimeBinder : Binder
	{
        public override bool CanBind(string name, Type type, IRequest request, RouteValues routeValues, object owner)
		{
			return type == typeof(DateTime);
		}

		public override BindingResult Bind(Type type, string name, string value, IRequest request, RouteValues routeValues, object owner)
		{
			if(string.IsNullOrWhiteSpace(value))
				return new BindingResult(DateTime.MinValue, BindingResult.ResultType.Default);

			DateTime datetime;
			if(DateTime.TryParseExact(value, new []
			{
				"dd/MM/yyyy",
				"dd/MM/yyyy HH:mm",
				"dd/MM/yyyy HH:mm:ss",
				"yyyy/MM/dd"
			}, new DateTimeFormatInfo(), DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out datetime))
				return new BindingResult(datetime);

			var dateTimeFormatInfo = new DateTimeFormatInfo();
			
			if(DateTime.TryParse(value, dateTimeFormatInfo, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out datetime))
				return new BindingResult(datetime);

			return new BindingResult(datetime, new BindingError("Invalid", name, value));
		}
	}
}