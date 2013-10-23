using System.Globalization;
using System.Text;

using Assman.Configuration;
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
                var culture = AssmanConfiguration.Current.Scripts.JsCompressionOverride.Culture;
                return JavaScriptCompressor.Compress(content, true, true, false, false, -1, Encoding.Default, culture);
            }

            return content;
        }
    }
}