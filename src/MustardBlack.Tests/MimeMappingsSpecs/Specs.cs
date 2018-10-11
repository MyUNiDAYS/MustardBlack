using FluentAssertions;
using NUnit.Framework;

namespace MustardBlack.Tests.MimeMappingsSpecs
{
	[TestFixture]
	public class Specs
	{
		[Then]
		public void Text()
		{
			MimeMapping.GetMimeMapping("\foo\bar.txt").Should().Be("text/plain");
		}

		[Then]
		public void FontWoff()
		{
			MimeMapping.GetMimeMapping("\foo\bar.woff").Should().Be("font/woff");
		}

		[Then]
		public void FontWoff2()
		{
			MimeMapping.GetMimeMapping("\foo\bar.woff2").Should().Be("font/woff2");
		}

		[Then]
		public void Gif()
		{
			MimeMapping.GetMimeMapping("\foo\bar.gif").Should().Be("image/gif");
		}

		[Then]
		public void JS()
		{
			MimeMapping.GetMimeMapping("\foo\bar.js").Should().Be("application/javascript");
		}

		[Then]
		public void SVG()
		{
			MimeMapping.GetMimeMapping("\foo\bar.SVG").Should().Be("image/svg+xml");
		}
	}
}