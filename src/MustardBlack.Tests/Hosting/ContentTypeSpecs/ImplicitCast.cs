using MustardBlack.Hosting;

namespace MustardBlack.Tests.Hosting.ContentTypeSpecs
{
    public class ImplicitCast : Specification
    {
        ContentType result;

        protected override void When()
        {
            this.result = "text/html; q=4";
        }

        [Then]
        public void ShouldCastToParsedContentType()
        {
            this.result.MediaType.ShouldEqual("text/html");
            this.result.Parameters["q"].ShouldEqual("4");
        }
    }
}