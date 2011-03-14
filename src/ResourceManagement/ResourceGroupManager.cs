using System;
using System.Collections.Generic;
using System.Linq;

using AlmWitt.Web.ResourceManagement.Configuration;

namespace AlmWitt.Web.ResourceManagement
{
	public class ResourceGroupManager : IResourceGroupManager
	{
		private readonly List<IResourceGroupTemplate> _templates = new List<IResourceGroupTemplate>();

		public void Add(IResourceGroupTemplate template)
		{
			_templates.Add(template);
		}

		public void Clear()
		{
			_templates.Clear();
		}

		public bool Any()
		{
			return _templates.Any();
		}

		public bool TryGetConsolidatedUrl(string virtualPath, out string consolidatedUrl)
		{
			foreach (var groupTemplate in _templates)
			{
				if (groupTemplate.TryGetConsolidatedUrl(virtualPath, out consolidatedUrl))
				{
					return true;
				}
			}

			consolidatedUrl = null;
			return false;
		}

		public bool IsConsolidatedUrl(string virtualPath)
		{
			return _templates.Any(t => t.MatchesConsolidatedUrl(virtualPath));
		}

		public GroupTemplateContext GetGroupTemplateOrDefault(string consolidatedUrl)
		{
			GroupTemplateContext groupTemplateContext = null;
			EachTemplate(c =>
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

		public IResourceGroup GetGroupOrDefault(string consolidatedUrl, ResourceMode mode, IResourceFinder finder)
		{
			var groupTemplateContext = GetGroupTemplateOrDefault(consolidatedUrl);
			if (groupTemplateContext == null)
				return null;

			var group = groupTemplateContext.FindGroupOrDefault(finder, consolidatedUrl, mode);
			if (group == null)
				return null;

			return group;
		}

		public void EachGroup(IEnumerable<IResource> allResources, ResourceMode mode, Action<IResourceGroup> handler)
		{
			EachTemplate(templateContext =>
			{
				var groups = templateContext.GetGroups(allResources, mode);
				foreach (var group in groups)
				{
					handler(group);
				}

				return true;
			});
		}

		private void EachTemplate(Func<GroupTemplateContext,bool> handler)
		{
			var excludeFilter = new CompositeResourceFilter();
			//add a filter to exclude all resources that match the consolidated url of a group
			excludeFilter.AddFilter(new ConsolidatedUrlFilter(_templates));
			foreach (var groupTemplate in _templates)
			{
				var templateContext = new GroupTemplateContext(groupTemplate, excludeFilter);
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
		private readonly IResourceFilter _excludeFilter;
		
		internal GroupTemplateContext(IResourceGroupTemplate groupTemplate, IResourceFilter excludeFilter)
		{
			GroupTemplate = groupTemplate;
			_excludeFilter = excludeFilter;
		}

		public IResourceGroupTemplate GroupTemplate { get; private set; }

		public ResourceType ResourceType
		{
			get { return GroupTemplate.ResourceType; }
		}

		public IResourceGroup FindGroupOrDefault(IResourceFinder finder, string consolidatedUrl, ResourceMode mode)
		{
			var resources = finder.FindResources(GroupTemplate.ResourceType);

			return FindGroupOrDefault(resources, consolidatedUrl, mode);
		}

		public IResourceGroup FindGroupOrDefault(IEnumerable<IResource> allResources, string consolidatedUrl, ResourceMode mode)
		{
			return (from @group in GetGroups(allResources, mode)
					where UrlType.ArePathsEqual(@group.ConsolidatedUrl, consolidatedUrl)
					select @group).SingleOrDefault();
		}

		public IEnumerable<IResourceGroup> GetGroups(IResourceFinder finder, ResourceMode mode)
		{
			var resources = finder.FindResources(GroupTemplate.ResourceType);
			return GetGroups(resources, mode);
		}

		public IEnumerable<IResourceGroup> GetGroups(IEnumerable<IResource> allResources, ResourceMode mode)
		{
			return GroupTemplate.GetGroups(allResources.Exclude(_excludeFilter), mode);
		}
	}

	public static class GroupTemplateExtensions
	{
		public static GroupTemplateContext WithContext(this IResourceGroupTemplate groupTemplate, IResourceFilter excludeFilter)
		{
			return new GroupTemplateContext(groupTemplate, excludeFilter);
		}

		public static GroupTemplateContext WithEmptyContext(this IResourceGroupTemplate groupTemplate)
		{
			return new GroupTemplateContext(groupTemplate, ResourceFilters.False);
		}
	}
}