using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AlmWitt.Web.ResourceManagement
{
	public class ResourceGroupTemplateCollection : Collection<IResourceGroupTemplate>
	{
		public void AddRange(IEnumerable<IResourceGroupTemplate> items)
		{
			foreach (var item in items)
			{
				Add(item);
			}
		}
		
		public GroupTemplateContext FindGroupTemplate(string consolidatedUrl)
		{
			GroupTemplateContext groupTemplateContext = null;
			ForEach(c =>
			{
				if (c.GroupTemplate.MatchesConsolidatedUrl(consolidatedUrl))
				{
					groupTemplateContext = c;
					return false;
				}
				return true;
			});
			
			return groupTemplateContext;
		}

		public void ForEach(Func<GroupTemplateContext,bool> handler)
		{
			var excludeFilter = new CompositeResourceFilter();
			foreach (var groupTemplate in this)
			{
				var templateContext = new GroupTemplateContext(groupTemplate)
				{
					ExcludeFilter = excludeFilter
				};
				if(!handler(templateContext))
					break;

				excludeFilter.AddFilter(groupTemplate);
			}
		}
	}

	public class GroupTemplateContext
	{
		public GroupTemplateContext(IResourceGroupTemplate groupTemplate)
		{
			GroupTemplate = groupTemplate;
			ExcludeFilter = ResourceFilters.False;
		}

		public IResourceGroupTemplate GroupTemplate { get; private set; }
		public IResourceFilter ExcludeFilter { get; set; }
	}
}