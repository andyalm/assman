using System;
using System.Web;

namespace Assman.DependencyManagement
{
	public static class DependencyManagerFactory
	{
		private static readonly IDependencyCache _dependencyCache = CreateDependencyCache();

		public static DependencyManager GetDependencyManager(IResourceFinder finder, IResourceGroupManager scriptGroups, IResourceGroupManager styleGroups)
		{
			return new DependencyManager(finder, _dependencyCache, scriptGroups, styleGroups);
		}

		private static IDependencyCache CreateDependencyCache()
		{
			if(HttpContext.Current != null)
			{
				return new HttpDependencyCache(() => new HttpContextWrapper(HttpContext.Current));
			}
			else
			{
				return new InMemoryDependencyCache();
			}
		}

        public static void ClearDependencyCache()
        {
            _dependencyCache.Clear();
        }
	}
}