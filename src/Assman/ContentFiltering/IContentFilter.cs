
namespace Assman.ContentFiltering
{
	/// <summary>
	/// An interface for filtering the content of a resource.
	/// </summary>
	public interface IContentFilter
	{
	    /// <summary>
	    /// Filters the given content.
	    /// </summary>
	    /// <param name="content"></param>
	    /// <param name="context"></param>
	    /// <returns>The filtered content.</returns>
	    string FilterContent(string content, ContentFilterContext context);
	}
}