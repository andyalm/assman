namespace AlmWitt.Web.ResourceManagement.Configuration
{
	public interface IResourceManagementPlugin
	{
		void Initialize(ResourceManagementContext context);
	}
}