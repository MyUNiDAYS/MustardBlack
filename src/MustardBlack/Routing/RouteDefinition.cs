namespace MustardBlack.Routing
{
	public class RouteDefinition
	{
		public string RoutePattern { get; set; }
		
		/// <summary>
		/// The request types handled by this route
		/// </summary>
		public RequestType RequestTypes { get; set; }

		/// <summary>
		/// Flag for routes that are cached
		/// </summary>
		public bool Localised { get; set; }

		/// <summary>
		/// Flag for routes that are personalised
		/// </summary>
		public bool Personalised { get; set; }
		
		public RouteDefinition()
		{
			this.Localised = true;
		}

		public RouteDefinition(string path)
		{
			this.RoutePattern = path;
			this.Localised = true;
		}

		public static implicit operator RouteDefinition(string value)
		{
			return new RouteDefinition
			{
				RoutePattern = value
			};
		}
	}
}
