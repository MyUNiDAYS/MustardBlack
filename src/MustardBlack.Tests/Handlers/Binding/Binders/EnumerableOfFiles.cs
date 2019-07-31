using System.Collections.Generic;
using System.IO;
using System.Linq;

using MustardBlack.Handlers.Binding;
using MustardBlack.Routing;
using NSubstitute;

namespace MustardBlack.Tests.Handlers.Binding.Binders
{
	public class EnumerableOfFiles : BindingSpecification
	{
		IEnumerable<IFile> target;
		IFile file1;
		IFile file2;

		protected override void Given()
		{
			base.Given();
			
			this.file1 = new TestFile();
			this.file2 = new TestFile();

			this.Request.HttpMethod = HttpMethod.Post;

			this.Request.Files.Returns(new Dictionary<string, IEnumerable<IFile>>(new Dictionary<string, IEnumerable<IFile>>
			{
				{ "files", new[] { this.file1, this.file2 } },
			}));
		}

		protected override void When()
		{
			var binder = BinderCollection.FindBinderFor(null, typeof (IEnumerable<IFile>), this.Request, new RouteValues(), null);
			var bindingResult = binder.Bind("files", typeof (IEnumerable<IFile>), this.Request, new RouteValues(), false, null);
			this.target = bindingResult.Object as IEnumerable<IFile>;
		}

		[Then]
		public void TheFilesShouldBeBound()
		{
			var array = this.target.ToArray();

			array[0].ShouldEqual(this.file1);
			array[1].ShouldEqual(this.file2);
		}

		public sealed class TestFile : IFile
		{
			public Stream InputStream { get; }
			public long ContentLength { get; }
			public string ContentType { get; }
			public string FileName { get; }
		}
	}
}