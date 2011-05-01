namespace Assman.ContentFiltering
{
	public class JSMinContentFilterFactory : IContentFilterFactory
	{
		public static JSMinContentFilterFactory GetInstance()
		{
			return new JSMinContentFilterFactory();
		}

		internal JSMinContentFilterFactory() { }

		public IContentFilter CreateFilter(IResourceGroup group, ResourceMode mode)
		{
			if (group.ShouldMinify(mode))
			{
				return new JSMinFilter();
			}
			else
			{
				return NullContentFilter.Instance;
			}
		}
	}
}