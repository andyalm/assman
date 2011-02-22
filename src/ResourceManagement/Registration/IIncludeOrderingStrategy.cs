namespace AlmWitt.Web.ResourceManagement.Registration
{
	public interface IIncludeOrderingStrategy
	{
		bool ShouldRegistryRenderInclude(string virtualPath, OrderingStategyContext context);
		void IncludeHasBeenRendered(string virtualPath, OrderingStategyContext context);
	}
}