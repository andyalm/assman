using System.Web;

using Assman.ContentFiltering;

using dotless.Core.configuration;

namespace Assman.Less
{
	public class LessContentFilter : IContentFilter
	{
	    private readonly ResourceContentSettings _settings;

	    public LessContentFilter(ResourceContentSettings settings)
	    {
	        _settings = settings;
	    }

	    public string FilterContent(string content)
		{
			var config = new DotlessConfiguration
			{
				MinifyOutput = _settings.Minify,
				Web = HttpContext.Current != null,
				LessSource = typeof(VirtualPathFileReader),
				CacheEnabled = false /* no need to cache as we let the Assman framework manage its own cache */
			};
			return dotless.Core.Less.Parse(content, config);
		}
	}
}