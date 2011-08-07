using Assman.Configuration;
using Assman.ContentFiltering;

using SassAndCoffee.Core.Compilers;

namespace Assman.SassAndCoffee
{
    public class CoffeeScriptAssmanPlugin : IAssmanPlugin
    {
        public void Initialize(AssmanContext context)
        {
            var coffeeScriptPipeline = new ContentFilterPipeline();
            coffeeScriptPipeline.Add(CoffeeScriptContentFilter.GetInstance());
            //TODO: Extend Assman.Core to support using whatever minifier we are using for .js files
            coffeeScriptPipeline.Add(CoffeeMinifyingFilter.GetInstance());
            context.MapExtensionToContentPipeline(".coffee", coffeeScriptPipeline);
            ResourceType.Script.AddFileExtension(".coffee");
        }
    }
}