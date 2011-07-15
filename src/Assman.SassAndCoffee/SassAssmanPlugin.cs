using System;

using Assman.Configuration;
using Assman.ContentFiltering;

using SassAndCoffee.Core.Compilers;

namespace Assman.SassAndCoffee
{
    public class SassAssmanPlugin : IAssmanPlugin
    {
        public void Initialize(AssmanContext context)
        {
            var pathResolver = VirtualPathResolver.GetInstance(AssmanConfiguration.Current.RootFilePath);

            var sassPipeline = new ContentFilterPipeline();
            sassPipeline.Add(CreateSassContentFilter(pathResolver));
            ResourceType.Stylesheet.AddFileExtension(".scss");
            context.MapExtensionToContentPipeline(".scss", sassPipeline);
            ResourceType.Stylesheet.AddFileExtension(".sass");
            context.MapExtensionToContentPipeline(".sass", sassPipeline);
        }

        public static IContentFilter CreateSassContentFilter()
        {
            var pathResolver = VirtualPathResolver.GetInstance(AssmanConfiguration.Current.RootFilePath);
            return CreateSassContentFilter(pathResolver);
        }

        public static IContentFilter CreateSassContentFilter(VirtualPathResolver pathResolver)
        {
            return new SassAndCoffeeFilter(CreateSassCompiler(pathResolver), pathResolver);
        }

        private static ISimpleFileCompiler CreateSassCompiler(VirtualPathResolver pathResolver)
        {
            var host = new AssmanSassAndCoffeeHost(pathResolver);
            var compiler = new SassFileCompiler();
            compiler.Init(host);

            return compiler;
        }
    }
}