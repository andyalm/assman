using System.Web;

using Assman.ContentFiltering;

using dotless.Core.configuration;

namespace Assman.Less
{
	public class LessContentFilter : IContentFilter
	{
		private readonly IResourceGroup _resourceGroup;
		private readonly ResourceMode _mode;

		public LessContentFilter(IResourceGroup resourceGroup, ResourceMode mode)
		{
			_resourceGroup = resourceGroup;
			_mode = mode;
		}

		public string FilterContent(string content)
		{
			var config = new DotlessConfiguration
			{
				MinifyOutput = _resourceGroup.ShouldMinify(_mode),
				Web = HttpContext.Current != null,
				LessSource = typeof(VirtualPathFileReader),
				CacheEnabled = false /* no need to cache as we let the Assman framework manage its own cache */
			};
			return dotless.Core.Less.Parse(content, config);
		}
	}
}