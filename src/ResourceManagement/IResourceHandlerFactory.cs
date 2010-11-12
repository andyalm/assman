using AlmWitt.Web.ResourceManagement.Configuration;

namespace AlmWitt.Web.ResourceManagement
{
	internal interface IResourceHandlerFactory
	{
		IResourceHandler CreateHandler(string path, ResourceManagementContext configContext, GroupTemplateContext groupTemplateContext);
	}

	internal class ResourceHandlerFactory : IResourceHandlerFactory
	{
		public IResourceHandler CreateHandler(string path, ResourceManagementContext configContext, GroupTemplateContext groupTemplateContext)
		{
			return new ResourceHandler(path, configContext, groupTemplateContext);
		}
	}
}