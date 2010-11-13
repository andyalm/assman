using AlmWitt.Web.ResourceManagement.ContentFiltering;

namespace AlmWitt.Web.ResourceManagement.Less
{
	public class LessContentFilterFactory : IContentFilterFactory
	{
		public IContentFilter CreateFilter(IResourceGroup group)
		{
			return new LessContentFilter(group);
		}
	}
}