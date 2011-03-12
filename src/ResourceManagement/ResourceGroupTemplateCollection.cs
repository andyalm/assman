using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using AlmWitt.Web.ResourceManagement.Configuration;

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
			excludeFilter.AddFilter(new ConsolidatedUrlFilter(this));
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

		private class ConsolidatedUrlFilter : IResourceFilter
		{
			private readonly IEnumerable<IResourceGroupTemplate> _templates;

			public ConsolidatedUrlFilter(IEnumerable<IResourceGroupTemplate> templates)
			{
				_templates = templates;
			}

			public bool IsMatch(IResource resource)
			{
				return _templates.Any(t => t.MatchesConsolidatedUrl(resource.VirtualPath));
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

		public IResourceGroup FindGroupOrDefault(IResourceFinder finder, string consolidatedUrl, ResourceMode mode)
		{
			var resources = finder
				.FindResources(GroupTemplate.ResourceType)
				.WhereNot(ExcludeFilter);

			return (from @group in GroupTemplate.GetGroups(resources, mode)
			        where UrlType.ArePathsEqual(@group.ConsolidatedUrl, consolidatedUrl)
			        select @group).SingleOrDefault();
		}
	}
}