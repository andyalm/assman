using System;
using System.IO;

using Crockford;

namespace Assman.ContentFiltering
{
    ///<summary>
    /// Defines a content filter for javascript that uses the JSMin library to minify it.
    ///</summary>
    public class JSMinFilter : IContentFilter
    {
        private readonly JavaScriptMinifier _minifier = new JavaScriptMinifier();

        /// <summary>
        /// Minifies javascript content using the JSMin library.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="context"></param>
        public string FilterContent(string content, ContentFilterContext context)
        {
            if (!context.Minify)
                return content;
            
            using (var input = new StringReader(content))
            {
                using (var output = new StringWriter())
                {
                    _minifier.Minify(input, output);
                    return output.ToString();
                }
            }
        }
    }
}