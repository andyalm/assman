using Assman.ContentFiltering;

namespace Assman.Less
{
	public class LessContentFilterFactory : IContentFilterFactory
	{
		public IContentFilter CreateFilter(ResourceContentSettings settings)
		{
			return new LessContentFilter(settings);
		}
	}
}