
namespace AlmWitt.Web.ResourceManagement.ContentFiltering
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
		/// <returns>The filtered content.</returns>
		string FilterContent(string content);
	}
}