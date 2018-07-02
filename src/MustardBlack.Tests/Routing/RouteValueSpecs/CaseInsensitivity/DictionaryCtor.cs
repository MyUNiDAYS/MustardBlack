using System.Collections.Generic;
using FluentAssertions;
using MustardBlack.Routing;

namespace MustardBlack.Tests.Routing.RouteValueSpecs.CaseInsensitivity
{
	public class DictionaryCtor : Specification
	{
		RouteValues subject;
		Dictionary<string, object> dictionary;

		protected override void Given()
		{
			this.dictionary = new Dictionary<string, object>
			{
				{ "FOO", "bar" }
			};
		}

		protected override void When()
		{
			this.subject = new RouteValues(dictionary);
		}

		[Then]
		public void ShouldContainKeyCaseInsensitively()
		{
			this.subject.ContainsKey("foo").Should().BeTrue();
		}
	}
}
