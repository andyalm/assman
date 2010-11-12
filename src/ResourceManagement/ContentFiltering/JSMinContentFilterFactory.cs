namespace AlmWitt.Web.ResourceManagement.ContentFiltering
{
	public class JSMinContentFilterFactory : IContentFilterFactory
	{
		public static JSMinContentFilterFactory GetInstance()
		{
			return new JSMinContentFilterFactory();
		}

		internal JSMinContentFilterFactory() { }

		public IContentFilter CreateFilter(IResourceGroupTemplate group)
		{
			if (group.Compress)
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