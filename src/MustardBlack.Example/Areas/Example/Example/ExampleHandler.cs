using MustardBlack.Handlers;
using MustardBlack.Results;

namespace MustardBlack.Example.Areas.Example.Example
{
	[Handle("/")]
	sealed class ExampleHandler : Handler
	{
		public IResult Get()
		{
			return new EmptyResult();
		}
	}
}
