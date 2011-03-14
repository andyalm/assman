using AlmWitt.Web.ResourceManagement.ContentFiltering;

namespace AlmWitt.Web.ResourceManagement
{
	public interface IContentFilterFactory
	{
		IContentFilter CreateFilter(IResourceGroup group, ResourceMode mode);
	}
}