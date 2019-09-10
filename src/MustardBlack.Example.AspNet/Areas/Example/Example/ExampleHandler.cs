using MustardBlack.Handlers;
using MustardBlack.Results;
using System;

namespace MustardBlack.Example.Areas.Example.Example
{
	[Handle("/")]
	sealed class ExampleHandler : Handler
	{
		public IResult Get()
		{
			return View("Index", new ExampleResource { ADate = DateTime.UtcNow });
		}
	}
}
