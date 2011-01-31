using System.Web;

using AlmWitt.Web.ResourceManagement.ContentFiltering;

using dotless.Core.configuration;

namespace AlmWitt.Web.ResourceManagement.Less
{
	public class LessContentFilter : IContentFilter
	{
		private readonly IResourceGroup _resourceGroup;

		public LessContentFilter(IResourceGroup resourceGroup)
		{
			_resourceGroup = resourceGroup;
		}

		public string FilterContent(string content)
		{
			var config = new DotlessConfiguration
			{
				MinifyOutput = _resourceGroup.Compress,
				Web = HttpContext.Current != null,
				LessSource = typeof(VirtualPathFileReader),
				CacheEnabled = false /* no need to cache as we let ResourceManagement framework manage its own cache */
			};
			return dotless.Core.Less.Parse(content, config);
		}
	}
}