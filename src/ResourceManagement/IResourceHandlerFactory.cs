namespace AlmWitt.Web.ResourceManagement
{
	internal interface IResourceHandlerFactory
	{
		IResourceHandler CreateHandler(string path, ResourceConsolidator consolidator, GroupTemplateContext groupTemplateContext);
	}

	internal class ResourceHandlerFactory : IResourceHandlerFactory
	{
		public IResourceHandler CreateHandler(string path, ResourceConsolidator consolidator, GroupTemplateContext groupTemplateContext)
		{
			return new ResourceHandler(path, consolidator, groupTemplateContext);
		}
	}
}