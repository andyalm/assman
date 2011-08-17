using System;

using Assman.ContentFiltering;

using SassAndCoffee.Core.Compilers;

namespace Assman.SassAndCoffee
{
    public class SassContentFilter : IContentFilter
    {
        private readonly SassFileCompiler _compiler;
        private readonly IPathResolver _pathResolver;

        public SassContentFilter(SassFileCompiler compiler, IPathResolver pathResolver)
        {
            _compiler = compiler;
            _pathResolver = pathResolver;
        }

        public string FilterContent(string content, ContentFilterContext context)
        {
            var filePath = _pathResolver.MapPath(context.ResourceVirtualPath);
            try
            {
                return _compiler.ProcessFileContent(new CompilableFileInfo
                {
                    Path = filePath,
                    Content = content
                });
            }
            catch (Exception ex)
            {
                //TODO: look into promoting this better exception handling into SassAndCoffee framework
                if(ex.ToString().Contains("Sass::SyntaxError"))
                {
                    dynamic dynamicException = ex;

                    throw new SassSyntaxException(dynamicException.to_s().ToString(), context.ResourceVirtualPath, ex);    
                }
                throw;
            }
        }
    }

    public class SassSyntaxException : Exception
    {
        public SassSyntaxException(string message, string filename, Exception innerException) : base(message, innerException)
        {
            Filename = filename;
        }

        public string Filename { get; private set; }
    }
}