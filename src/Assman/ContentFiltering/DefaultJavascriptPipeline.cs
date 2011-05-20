namespace Assman.ContentFiltering
{
    public static class DefaultJavascriptPipeline
    {
        public static ContentFilterPipeline Create()
        {
            var pipeline = new ContentFilterPipeline();
            pipeline.Add(new JSMinFilter());

            return pipeline;
        }
    }
}