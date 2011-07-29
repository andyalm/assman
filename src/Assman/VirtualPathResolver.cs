using System;
using System.IO;

namespace Assman
{
    public interface IPathResolver
    {
        string MapPath(string path);
    }

    public class VirtualPathResolver : IPathResolver
    {
        public static IPathResolver GetInstance(string appPath)
        {
            return new VirtualPathResolver(appPath);
        }

        private readonly string _appPath;

        internal VirtualPathResolver(string appPath)
        {
            _appPath = appPath;
        }

        public string AppPath
        {
            get { return _appPath; }
        }

        public string MapPath(string virtualPath)
        {
            if (virtualPath.StartsWith("~"))
                virtualPath = virtualPath.Substring(1);
            if (virtualPath.StartsWith("/"))
                virtualPath = virtualPath.Substring(1);
            virtualPath = virtualPath.Replace('/', '\\');

            return Path.Combine(AppPath, virtualPath);
        }
    }
}