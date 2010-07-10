using System;

using AlmWitt.Web.ResourceManagement.Configuration;
using AlmWitt.Web.ResourceManagement.IO;

namespace AlmWitt.Web.ResourceManagement.Less
{
    public class LessResourceFinderFactory : IResourceFinderFactory
    {
        public IResourceFinder CreateFinder()
        {
            var rootDirectory = ResourceManagementConfiguration.Current.RootFilePath;

            return new LessResourceFinder(rootDirectory, FileFinder.GetInstance());
        }
    }
}