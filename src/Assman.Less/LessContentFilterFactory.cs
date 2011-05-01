using Assman.ContentFiltering;

namespace Assman.Less
{
	public class LessContentFilterFactory : IContentFilterFactory
	{
		public IContentFilter CreateFilter(IResourceGroup group, ResourceMode mode)
		{
			return new LessContentFilter(group, mode);
		}
	}
}