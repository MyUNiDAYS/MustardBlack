using MustardBlack.Handlers;
using MustardBlack.Results;

namespace MustardBlack.Example.AspNetCore.Areas.Example.Example
{
	[Handle("/")]
	sealed class ExampleHandler : Handler
	{
		public IResult Get()
		{
			return this.View("Index");
		}

		public IResult Post(Resource resource)
		{
			return this.View("Posted");
		}
	}
}
