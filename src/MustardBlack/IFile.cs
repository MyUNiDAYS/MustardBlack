using System.IO;

namespace MustardBlack
{
	public interface IFile
	{
		Stream InputStream { get; }
		int ContentLength { get; }
		string ContentType { get; }
		string FileName { get; }
	}
}
