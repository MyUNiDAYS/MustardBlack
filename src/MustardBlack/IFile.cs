using System.IO;

namespace MustardBlack
{
	public interface IFile
	{
		Stream InputStream { get; }
		long ContentLength { get; }
		string ContentType { get; }
		string FileName { get; }
	}
}
