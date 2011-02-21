using System.Web;

namespace AlmWitt.Web.ResourceManagement
{
	public static class DependencyManagerFactory
	{
		private static readonly IDependencyCache _dependencyCache = CreateDependencyCache();
		
		public static DependencyManager GetDependencyManager(IResourceFinder finder)
		{
			return new DependencyManager(finder, _dependencyCache);
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
	}
}