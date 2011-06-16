using Assman.ContentFiltering;

using Yahoo.Yui.Compressor;

namespace Assman.YuiCompressor
{
    public class YuiCompressorCssContentFilter : IContentFilter
    {
        public string FilterContent(string content, ContentFilterContext context)
        {
            if(context.Minify)
            {
                return CssCompressor.Compress(content);
            }

            return content;
        }
    }
}