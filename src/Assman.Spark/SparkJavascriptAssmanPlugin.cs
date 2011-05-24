using System;

using Assman.Configuration;

using Spark;

namespace Assman.Spark
{
	public class SparkJavascriptAssmanPlugin : IAssmanPlugin
	{
		public void Initialize(AssmanContext context)
		{
			var finder = CreateFinder();

			context.AddFinder(finder);
		}

		private SparkResourceFinder CreateFinder()
		{
			var assemblies = AssmanConfiguration.Current.Assemblies.GetAssemblies();
			var contentFetcher = CreateContentFetcher();
			var actionFinder = CreateActionFinder();
			
			return new SparkResourceFinder(assemblies, contentFetcher, actionFinder);
		}

		protected virtual ISparkJavascriptActionFinder CreateActionFinder()
		{
			return new AttributeBasedActionFinder();
		}

		protected virtual ISparkResourceContentFetcher CreateContentFetcher()
		{
			var sparkSettings = GetSparkSettings();
			
			return new SparkResourceContentFetcher(sparkSettings);
		}

		protected virtual ISparkSettings GetSparkSettings()
		{
			return new SparkSettings();
		}
	}
}