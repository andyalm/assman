using System;

using Assman.ContentFiltering;

using Yahoo.Yui.Compressor;

namespace Assman.YuiCompressor
{
    public class YuiCompressorCssContentFilter : IContentFilter
    {
        public string FilterContent(string content, ContentFilterContext context)
        {
            if (string.IsNullOrEmpty(content)) throw new ArgumentException("File content cannot be empty: " + context.ResourceVirtualPath);
            if(context.Minify)
            {
                try
                {
                    return CssCompressor.Compress(content);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("Css Yui compressor was unable to compress the following file: " + context.ResourceVirtualPath, ex);
                }
            }

            return content;
        }
    }
}