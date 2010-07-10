using System;

namespace AlmWitt.Web.ResourceManagement.Less
{
    public class LessResource : FileResource
    {
        public LessResource(string fullFilePath, string baseDirectory) : base(fullFilePath, baseDirectory)
        {
            
        }

        public override string GetContent()
        {
            var rawContent = base.GetContent();

            return dotless.Core.Less.Parse(rawContent);
        }
    }
}