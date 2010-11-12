using System;
using System.Collections.ObjectModel;

namespace AlmWitt.Web.ResourceManagement.Configuration
{
	public class ResourceGroupCollection : KeyedCollection<string,IResourceGroup>
	{
		protected override string GetKeyForItem(IResourceGroup item)
		{
			return item.ConsolidatedUrl;
		}

		public void ProcessEach(ResourceGroupProcessor processor)
		{
			var previousGroupFilters = new CompositeResourceFilter();
			foreach (var group in this)
			{
				processor(group, previousGroupFilters.Clone());
				previousGroupFilters.AddFilter(group);
			}
		}
	}

	public delegate void ResourceGroupProcessor(IResourceGroup group, IResourceFilter excludeFilter);
}