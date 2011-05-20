namespace Assman.ContentFiltering
{
    public class ContentFilterContext
    {
        public IResourceGroup Group { get; set; }
        public string ResourceVirtualPath { get; set; }
        public bool Minify { get; set; }
        public ResourceMode Mode { get; set; }
    }
}