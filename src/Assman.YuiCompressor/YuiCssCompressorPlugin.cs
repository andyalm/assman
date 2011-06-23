using Assman.Configuration;

namespace Assman.YuiCompressor
{
    public class YuiCssCompressorPlugin : IAssmanPlugin
    {
        public void Initialize(AssmanContext context)
        {
            var cssPipeline = context.GetContentPipelineForExtension(".css");
            cssPipeline.Add(new YuiCompressorCssContentFilter());
        }
    }
}