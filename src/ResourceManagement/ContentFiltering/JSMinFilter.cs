using System;
using System.IO;

using Crockford;

namespace AlmWitt.Web.ResourceManagement.ContentFiltering
{
    ///<summary>
    /// Defines a content filter for javascript that uses the JSMin library to minify it.
    ///</summary>
    public class JSMinFilter : IContentFilter
    {
        private readonly JavaScriptMinifier _minifier = new JavaScriptMinifier();

        #region IContentFilter Members

        /// <summary>
        /// Minifies javascript content using the JSMin library.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public string FilterContent(string content)
        {
            using (var input = new StringReader(content))
            {
                using (var output = new StringWriter())
                {
                    _minifier.Minify(input, output);
                    return output.ToString();
                }
            }
        }

        #endregion
    }
}