using System;
using System.Web;

using Assman.ContentFiltering;

using dotless.Core.configuration;

namespace Assman.dotLess
{
	public class LessContentFilter : IContentFilter
	{
	    public string FilterContent(string content, ContentFilterContext context)
		{
			var config = new DotlessConfiguration
			{
				MinifyOutput = context.Minify,
				Web = HttpContext.Current != null,
				LessSource = typeof(VirtualPathFileReader),
				CacheEnabled = false /* no need to cache as we let the Assman framework manage its own cache */
			};
			return dotless.Core.Less.Parse(content, config);
		}
	}
}