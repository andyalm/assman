using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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
			//add a filter to exclude all resources that match the consolidated url of a group
			excludeFilter.AddFilter(new GroupPathFilter(this.Select<IResourceGroupTemplate,Func<string,bool>>(t => t.MatchesConsolidatedUrl).ToList()));
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

		private class GroupPathFilter : IResourceFilter
		{
			private readonly IEnumerable<Func<string,bool>> _isMatchPredicates;

			public GroupPathFilter(IEnumerable<Func<string,bool>> isMatchPredicates)
			{
				_isMatchPredicates = isMatchPredicates;
			}

			public bool IsMatch(IResource resource)
			{
				return _isMatchPredicates.Any(predicate => predicate(resource.VirtualPath));
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