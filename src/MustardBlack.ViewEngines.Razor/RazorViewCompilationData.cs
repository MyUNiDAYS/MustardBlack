namespace MustardBlack.ViewEngines.Razor
{
	public sealed class RazorViewCompilationData
	{
		public string Namespace { get; set; }
		public string ClassName { get; set; }
		public string FilePath { get; set; }
		public string ViewContents { get; set; }
	}
}
