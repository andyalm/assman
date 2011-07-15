using System;

using SassAndCoffee.Core;

namespace Assman.SassAndCoffee
{
    public class AssmanSassAndCoffeeHost : ICompilerHost
    {
        private readonly VirtualPathResolver _pathResolver;

        public AssmanSassAndCoffeeHost(VirtualPathResolver pathResolver)
        {
            _pathResolver = pathResolver;
        }

        public string MapPath(string path)
        {
            return _pathResolver.MapPath(path);
        }

        public string ApplicationBasePath
        {
            get { return _pathResolver.AppPath; }
        }
    }
}