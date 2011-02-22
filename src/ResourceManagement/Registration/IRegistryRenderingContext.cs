namespace AlmWitt.Web.ResourceManagement.Registration
{
	public interface IRegistryRenderingContext
	{
		string ResolveUrl(string virtualPath);
		bool ShouldRenderInclude(string virtualPath);
		void MarkIncludeAsRendered(string virtualPath);
	}
}