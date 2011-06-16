using Assman.Configuration;
using Assman.ContentFiltering;

namespace Assman.YuiCompressor
{
    public class YuiCompressorPlugin : IAssmanPlugin
    {
        public void Initialize(AssmanContext context)
        {
            var jsPipeline = context.GetContentPipelineForExtension(".js");
            jsPipeline.Remove<JSMinFilter>();
            jsPipeline.Add(new YuiCompressorJavaScriptContentFilter());

            var cssPipeline = context.GetContentPipelineForExtension(".css");
            cssPipeline.Add(new YuiCompressorCssContentFilter());
        }
    }
}