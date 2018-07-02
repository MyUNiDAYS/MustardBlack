using System.Net;
using FluentAssertions;
using MustardBlack.Results;

namespace MustardBlack.Tests.ModRewrite.NonRedirects
{
	public class NotFound : ModRewriteSpecification
	{
		protected override string Rules => @"

RewriteRule bar - [R=404]

";

		protected override string RequestUrl => "https://unidays.test/bar";
		
		[Then]
		public void Should404()
		{
			var result = this.handledResponse as EmptyResult;
			result.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}
	}
}
