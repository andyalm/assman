namespace AlmWitt.Web.ResourceManagement.ContentFiltering
{
	public interface IContentFilterFactory
	{
		IContentFilter CreateFilter(IResourceGroupTemplate group);
	}
}