using Assman.ContentFiltering;

using Yahoo.Yui.Compressor;

namespace Assman.YuiCompressor
{
    public class YuiCompressorJavaScriptContentFilter : IContentFilter
    {
        public string FilterContent(string content, ContentFilterContext context)
        {
            if(context.Minify)
            {
                return JavaScriptCompressor.Compress(content);
            }

            return content;
        }
    }
}