using System;
using System.Collections.Generic;
using System.Linq;

namespace AlmWitt.Web.ResourceManagement
{
	public class ResourceGroupManager : IResourceGroupManager
	{
		public static IResourceGroupManager GetInstance()
		{
			return new CachingResourceGroupManager(new ResourceGroupManager());
		}
		
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

		public string GetResourceUrl(string virtualPath)
		{
			foreach (var groupTemplate in _templates)
			{
				string consolidatedUrl;
				if (groupTemplate.TryGetConsolidatedUrl(virtualPath, out consolidatedUrl))
				{
					return consolidatedUrl;
				}
			}

			return virtualPath;
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
				var templateContext = groupTemplate.WithContext(excludeFilter);
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

		internal class CachingResourceGroupManager : IResourceGroupManager
		{
			private readonly IResourceGroupManager _inner;
			private readonly ThreadSafeInMemoryCache<string, string> _resolvedResourceUrls = new ThreadSafeInMemoryCache<string, string>(StringComparer.OrdinalIgnoreCase);

			public CachingResourceGroupManager(IResourceGroupManager inner)
			{
				_inner = inner;
			}

			public void Add(IResourceGroupTemplate template)
			{
				_inner.Add(template);
			}

			public void Clear()
			{
				_inner.Clear();
			}

			public bool Any()
			{
				return _inner.Any();
			}

			public string GetResourceUrl(string virtualPath)
			{
				return _resolvedResourceUrls.GetOrAdd(virtualPath, () => _inner.GetResourceUrl(virtualPath));
			}

			public bool IsConsolidatedUrl(string virtualPath)
			{
				return _inner.IsConsolidatedUrl(virtualPath);
			}

			public GroupTemplateContext GetGroupTemplateOrDefault(string consolidatedUrl)
			{
				return _inner.GetGroupTemplateOrDefault(consolidatedUrl);
			}

			public IResourceGroup GetGroupOrDefault(string consolidatedUrl, ResourceMode mode, IResourceFinder finder)
			{
				//TODO: Add caching
				return _inner.GetGroupOrDefault(consolidatedUrl, mode, finder);
			}

			public void EachGroup(IEnumerable<IResource> allResources, ResourceMode mode, Action<IResourceGroup> handler)
			{
				_inner.EachGroup(allResources, mode, handler);
			}
		}
	}
}