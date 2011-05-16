namespace Assman.ContentFiltering
{
	public class JSMinContentFilterFactory : IContentFilterFactory
	{
		public static JSMinContentFilterFactory GetInstance()
		{
			return new JSMinContentFilterFactory();
		}

		internal JSMinContentFilterFactory() { }

		public IContentFilter CreateFilter(ResourceContentSettings settings)
		{
			if (settings.Minify)
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