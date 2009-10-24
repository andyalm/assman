using System;
using System.IO;

using Crockford;

namespace AlmWitt.Web.ResourceManagement.ContentFiltering
{
    ///<summary>
    /// Defines a content filter for javascript that uses the JSMin library to compress it.
    ///</summary>
    public class JSMinFilter : IContentFilter
    {
        private readonly JavaScriptMinifier minifier = new JavaScriptMinifier();

        #region IContentFilter Members

        /// <summary>
        /// Compresses javascript content using the JSMin library.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public string FilterContent(string content)
        {
            using (StringReader input = new StringReader(content))
            {
                using (StringWriter output = new StringWriter())
                {
                    minifier.Minify(input, output);
                    return output.ToString();
                }
            }
        }

        #endregion
    }
}