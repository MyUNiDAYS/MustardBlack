using System;
using System.Globalization;
using MustardBlack.Hosting;
using MustardBlack.Routing;
using Newtonsoft.Json;

namespace MustardBlack.Handlers.Binding.Binders
{
	public sealed class JsonBinder : Binder
	{
		public override bool CanBind(string name, Type type, IRequest request, RouteValues routeValues, object owner)
		{
			return !string.IsNullOrEmpty(request.ContentType) &&
				request.ContentType.ToLowerInvariant() == "application/json" &&
				!request.HttpMethod.IsSafe() &&
				!type.IsValueType;
		}

		public override BindingResult Bind(Type type, string name, string value, IRequest request, RouteValues routeValues, object owner)
		{
			var data = JsonConvert.DeserializeObject(value, type, new EmailAddressJsonConvert());

			return new BindingResult(data);
		}

		sealed class EmailAddressJsonConvert : JsonConverter
		{
			public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
			{
				throw new NotSupportedException();
			}

			public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
			{
				if (reader.TokenType == JsonToken.Null)
					return null;

				if (reader.TokenType != JsonToken.String)
					return null;

				var value = (string)reader.Value;

				var path = reader.Path.Split('.');
				var propertyName = path[path.Length - 1].ToLowerInvariant();

				if (propertyName.IndexOf("email", StringComparison.InvariantCultureIgnoreCase) == -1)
					return value;

				// sense check, there may be some field that we've incorrectly caught, potentially
				if (value.IndexOf('@') <= -1)
					return value;

				var idn = new IdnMapping();
				try
				{
					var parts = value.Split('@');
					var domainName = idn.GetAscii(parts[1]);
					return parts[0] + '@' + domainName;
				}
				catch (ArgumentException)
				{
					return value;
				}
			}

			public override bool CanConvert(Type objectType)
			{
				return objectType == typeof(string);
			}
		}
	}
}
