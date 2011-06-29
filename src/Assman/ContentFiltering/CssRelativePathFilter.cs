using System;
using System.Text.RegularExpressions;

namespace Assman.ContentFiltering
{
    public class CssRelativePathFilter : IContentFilter
    {
        private static readonly Regex _urlRegex = new Regex(@"url\((?<url>[^)]+)\)", RegexOptions.Compiled);
        
        public string FilterContent(string content, ContentFilterContext context)
        {
            if (context.Group == null)
                return content;
            
            return _urlRegex.Replace(content, m => FixPath(m, context));
        }

        private string FixPath(Match match, ContentFilterContext context)
        {
            var url = match.Groups["url"].Value;
            if (url.StartsWith("/"))
                return match.Value;

            var consolidatedUri = CreateUri(context.Group.ConsolidatedUrl);
            var uri = CreateUri(url.ToAppRelativePath(context.ResourceVirtualPath));

            var fixedRelativePath = consolidatedUri.MakeRelativeUri(uri).ToString();

            return String.Format("url({0})", fixedRelativePath);
        }

        private Uri CreateUri(string virtualPath)
        {
            return new Uri("http://www.mywebsite.com" + virtualPath.Substring(1));
        }
    }
}