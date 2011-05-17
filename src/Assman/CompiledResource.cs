using System.IO;

namespace Assman
{
    public class CompiledResource
    {
        public IResource Resource { get; set; }
        public ResourceMode Mode { get; set; }
        public string CompiledContent { get; set; }
        
        private string _compiledPath;
        public string CompiledPath
        {
            get
            {
                if(_compiledPath == null && Resource != null)
                {
                    _compiledPath = CalculateCompiledPath();
                }
                
                return _compiledPath;
            }
            set { _compiledPath = value; }
        }

        private string CalculateCompiledPath()
        {
            var lastDotIndex = Resource.VirtualPath.LastIndexOf(".");
            var beforePeriod = Resource.VirtualPath.Substring(0, lastDotIndex);
            var afterPeriod = Resource.VirtualPath.Substring(lastDotIndex + 1);

            return beforePeriod + ".min." + afterPeriod;
        }

        public void WriteToFile(string path)
        {
            //ensure the destination directory exists
            string directory = Path.GetDirectoryName(path);
            Directory.CreateDirectory(directory);

            using (var writer = new StreamWriter(path, append:false))
            {
                writer.Write(CompiledContent);
            }
        }
    }
}