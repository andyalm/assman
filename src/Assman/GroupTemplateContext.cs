using System;
using System.Collections.Generic;
using System.Linq;

namespace Assman
{
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
			var resources = finder.FindResources(GroupTemplate.ResourceType).ToArray();

			return FindGroupOrDefault(resources, consolidatedUrl, mode);
		}

		public IResourceGroup FindGroupOrDefault(IEnumerable<IResource> allResources, string consolidatedUrl, ResourceMode mode)
		{
			return (from @group in GetGroups(allResources, mode)
			        where @group.ConsolidatedUrl.EqualsVirtualPath(consolidatedUrl)
			        select @group).SingleOrDefault();
		}

		public IEnumerable<IResourceGroup> GetGroups(IResourceFinder finder, ResourceMode mode)
		{
			var resources = finder.FindResources(GroupTemplate.ResourceType);
			return GetGroups(resources, mode);
		}

		public IEnumerable<IResourceGroup> GetGroups(IEnumerable<IResource> allResources, ResourceMode mode)
		{
			return GroupTemplate.GetGroups(allResources.Exclude(_excludeFilter).ToArray(), mode);
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