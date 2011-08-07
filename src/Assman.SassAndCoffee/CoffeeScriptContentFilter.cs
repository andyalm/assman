using Assman.ContentFiltering;

using SassAndCoffee.Core.Compilers;

namespace Assman.SassAndCoffee
{
    public class CoffeeScriptContentFilter : IContentFilter
    {
        public static CoffeeScriptContentFilter GetInstance()
        {
            return new CoffeeScriptContentFilter(new CoffeeScriptCompiler());
        }
        
        private readonly CoffeeScriptCompiler _coffeeScriptCompiler;

        public CoffeeScriptContentFilter(CoffeeScriptCompiler coffeeScriptCompiler)
        {
            _coffeeScriptCompiler = coffeeScriptCompiler;
        }

        public string FilterContent(string content, ContentFilterContext context)
        {
            return _coffeeScriptCompiler.Compile(content);
        }
    }
}