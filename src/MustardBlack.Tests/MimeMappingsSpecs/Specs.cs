
namespace MustardBlack.Tests.MimeMappingsSpecs
{
	public class Specs
	{
		[Then]
		public void Text()
		{
			MimeMapping.GetMimeMapping("\foo\bar.txt").ShouldEqual("text/plain");
		}

		[Then]
		public void FontWoff()
		{
			MimeMapping.GetMimeMapping("\foo\bar.woff").ShouldEqual("font/woff");
		}

		[Then]
		public void FontWoff2()
		{
			MimeMapping.GetMimeMapping("\foo\bar.woff2").ShouldEqual("font/woff2");
		}

		[Then]
		public void Gif()
		{
			MimeMapping.GetMimeMapping("\foo\bar.gif").ShouldEqual("image/gif");
		}

		[Then]
		public void JS()
		{
			MimeMapping.GetMimeMapping("\foo\bar.js").ShouldEqual("application/javascript");
		}

		[Then]
		public void SVG()
		{
			MimeMapping.GetMimeMapping("\foo\bar.SVG").ShouldEqual("image/svg+xml");
		}
	}
}