using System;
using System.IO;

using Assman.ContentFiltering;

using SassAndCoffee.Core.Compilers;

namespace Assman.SassAndCoffee
{
    public class SassContentFilter : IContentFilter
    {
        private readonly SassFileCompiler _compiler;

        public SassContentFilter(SassFileCompiler compiler)
        {
            _compiler = compiler;
        }

        public string FilterContent(string content, ContentFilterContext context)
        {
            return _compiler.ProcessContent(content, Path.GetExtension(context.ResourceVirtualPath));
        }
    }
}