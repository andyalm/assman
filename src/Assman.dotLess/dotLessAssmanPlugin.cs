using System;

using Assman.Configuration;
using Assman.ContentFiltering;
using Assman.DependencyManagement;

namespace Assman.dotLess
{
    public class dotLessAssmanPlugin : IAssmanPlugin
    {
        private const string LessFileExtension = ".less";
        
        public void Initialize(AssmanContext context)
        {
            ResourceType.Stylesheet.AddFileExtension(LessFileExtension);
            var lessPipeline = new ContentFilterPipeline();
            lessPipeline.Add(new LessContentFilter());
            lessPipeline.Add(CssRelativePathFilter.GetInstance());
            context.MapExtensionToContentPipeline(LessFileExtension, lessPipeline);
            context.MapExtensionToDependencyProvider(LessFileExtension, CssDependencyProvider.GetInstance());
        }
    }
}