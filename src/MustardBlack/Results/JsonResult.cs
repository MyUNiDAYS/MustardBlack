using System;
using System.Net;
using Newtonsoft.Json;

namespace MustardBlack.Results
{
	public sealed class JsonResult : Result
	{
		public readonly object Data;

		/// <summary>
		/// Settings for JSON serialisation. These will only be used when Data is not already a string.
		/// </summary>
		public JsonSerializerSettings SerializerSettings { get; set; }

		/// <summary>
		/// Creates a new JSON Result from a string of JSON
		/// </summary>
		/// <param name="data">Assumed to be valid JSON</param>
		/// <param name="statusCode"></param>
		public JsonResult(string data, HttpStatusCode statusCode = HttpStatusCode.OK)
		{
			if (data == null)
				throw new ArgumentException("You must provide data", nameof(data));

			if (string.IsNullOrWhiteSpace(data))
				throw new ArgumentException("You must provide data", nameof(data));

			this.Data = data;
			this.StatusCode = statusCode;
		}

		/// <summary>
		/// Creates a new JSON Result from an object which will be serialised
		/// </summary>
		/// <param name="data">An object to serialise as JSON</param>
		/// <param name="statusCode"></param>
		public JsonResult(object data, HttpStatusCode statusCode = HttpStatusCode.OK)
		{
			if(data == null)
				throw new ArgumentNullException("You must provide data", nameof(data));

			this.Data = data;
			this.StatusCode = statusCode;
		}
	}
}
