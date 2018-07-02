namespace MustardBlack.ViewEngines
{
	public interface IView
	{
		void SetHelpers(HtmlHelper htmlHelper, UrlHelper urlHelper);
		HtmlHelper Html { get; }
		UrlHelper Url { get; }
	}
}