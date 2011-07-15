using System;

using Assman.ContentFiltering;

using SassAndCoffee.Core.Compilers;

namespace Assman.SassAndCoffee
{
    public class SassAndCoffeeFilter : IContentFilter
    {
        private readonly ISimpleFileCompiler _compiler;
        private readonly VirtualPathResolver _pathResolver;

        public SassAndCoffeeFilter(ISimpleFileCompiler compiler, VirtualPathResolver pathResolver)
        {
            _compiler = compiler;
            _pathResolver = pathResolver;
        }

        public string FilterContent(string content, ContentFilterContext context)
        {
            var filePhysicalPath = _pathResolver.MapPath(context.ResourceVirtualPath);
            return _compiler.ProcessFileContent(filePhysicalPath);
        }
    }
}