namespace AlmWitt.Web.ResourceManagement.ContentFiltering
{
	public class NullContentFilterFactory : IContentFilterFactory
	{
		private static readonly IContentFilterFactory _instance = new NullContentFilterFactory();

		public static IContentFilterFactory Instance
		{
			get { return _instance; }
		}

		private NullContentFilterFactory() {}
		
		public IContentFilter CreateFilter(IResourceGroup group)
		{
			return NullContentFilter.Instance;
		}
	}
}