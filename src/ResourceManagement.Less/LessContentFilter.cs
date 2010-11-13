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
				MinifyOutput = _resourceGroup.Compress
			};
			return dotless.Core.Less.Parse(content, config);
		}
	}
}