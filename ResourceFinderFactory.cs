using System;
using System.IO;
using System.Reflection;

namespace AlmWitt.Web.ResourceManagement
{
	/// <summary>
	/// Constructs <see cref="IResourceFinder"/>s.
	/// </summary>
	public static class ResourceFinderFactory
	{
	    /// <summary>
	    /// Returns a no-op <see cref="IResourceFinder"/>. (null object pattern)
	    /// </summary>
        public static IResourceFinder Null
	    {
            get { return NullResourceFinder.Instance; }
	    }
        
        /// <summary>
        /// Creates a <see cref="IResourceFinder"/> that will find resources in the given file system directory (and sub-directories).
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static IResourceFinder GetInstance(string directory)
		{
		    return new FileResourceFinder(directory);
		}

        /// <summary>
        /// Creates a <see cref="IResourceFinder"/> that will find embedded resources in the given assembly.
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static IResourceFinder GetInstance(Assembly assembly)
        {
            return new EmbeddedResourceFinder(assembly);
        }

        private class NullResourceFinder : IResourceFinder
        {
            private static readonly IResourceFinder _instance = new NullResourceFinder();

            public static IResourceFinder Instance
            {
                get { return _instance; }
            }

            private NullResourceFinder()
            {
            }

            public ResourceCollection FindResources(string extension)
            {
                return new ResourceCollection();
            }
        }
	}
}