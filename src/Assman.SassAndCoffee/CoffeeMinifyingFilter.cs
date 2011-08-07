using Assman.ContentFiltering;

using SassAndCoffee.Core.Compilers;

namespace Assman.SassAndCoffee
{
    public class CoffeeMinifyingFilter : IContentFilter
    {
        public static CoffeeMinifyingFilter GetInstance()
        {
            return new CoffeeMinifyingFilter(new MinifyingCompiler());
        }
        
        private readonly MinifyingCompiler _minifier;

        public CoffeeMinifyingFilter(MinifyingCompiler minifier)
        {
            _minifier = minifier;
        }

        public string FilterContent(string content, ContentFilterContext context)
        {
            if (context.Minify)
                return _minifier.Compile(content);
            else
                return content;
        }
    }
}