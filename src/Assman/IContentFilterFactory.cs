using Assman.ContentFiltering;

namespace Assman
{
	public interface IContentFilterFactory
	{
		IContentFilter CreateFilter(IResourceGroup group, ResourceMode mode);
	}
}