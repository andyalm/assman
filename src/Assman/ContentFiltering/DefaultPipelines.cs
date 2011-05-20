namespace Assman.ContentFiltering
{
    public static class DefaultPipelines
    {
        /// <summary>
        /// Constructs a new <see cref="ContentFilterPipeline"/> that is used by default for javascript files.
        /// </summary>
        public static ContentFilterPipeline Javascript()
        {
            var pipeline = new ContentFilterPipeline();
            pipeline.Add(new JSMinFilter());

            return pipeline;
        }

        /// <summary>
        /// Constructs a new <see cref="ContentFilterPipeline"/> that is used by default for css files.
        /// </summary>
        public static ContentFilterPipeline Css()
        {
            var pipeline = new ContentFilterPipeline();
            pipeline.Add(new CssRelativePathFilter());

            return pipeline;
        }
    }
}