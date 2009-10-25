using System;
using System.Collections.Generic;

using AlmWitt.Web.ResourceManagement.Configuration;

using Spark;

namespace AlmWitt.Web.ResourceManagement.Spark
{
	public class SparkResourceFinderFactory : IResourceFinderFactory
	{
		private Dictionary<Type,object> _objectCache = new Dictionary<Type, object>();

		public IResourceFinder CreateFinder()
		{
			var assemblies = ResourceManagementConfiguration.Current.Assemblies.GetAssemblies();

			return new SparkResourceFinder(assemblies, ContentFetcher, ActionFinder);
		}

		protected virtual ISparkJavascriptActionFinder ActionFinder
		{
			get { return CachedInstance(() => new AttributeBasedActionFinder()); }
		}

		protected virtual ISparkResourceContentFetcher ContentFetcher
		{
			get { return CachedInstance(() => new SparkResourceContentFetcher(SparkSettings)); }
		}

		protected virtual ISparkSettings SparkSettings
		{
			get
			{
				return CachedInstance(() =>
				{
					return new SparkSettings();
				});
			}
		}

		private T CachedInstance<T>(Func<T> create)
		{
			var key = typeof(T);
			if(_objectCache.ContainsKey(key))
			{
				return (T) _objectCache[key];
			}

			var value = create();
			_objectCache[key] = value;

			return value;
		}
	}
}