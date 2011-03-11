using AlmWitt.Web.ResourceManagement.Configuration;

namespace AlmWitt.Web.ResourceManagement.Less
{
	public class LessResourceManagementPlugin : IResourceManagementPlugin
	{
		private const string LessFileExtension = ".less";
		
		public void Initialize(ResourceManagementContext context)
		{
			ResourceType.Css.AddFileExtension(LessFileExtension);
			context.MapExtensionToFilter(LessFileExtension, new LessContentFilterFactory());
			context.MapExtensionToDependencyProvider(LessFileExtension, CssDependencyProvider.GetInstance());
		}
	}
}