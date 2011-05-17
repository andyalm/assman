using System;
using System.Collections.Generic;
using System.Linq;

using Assman.Configuration;
using Assman.ContentFiltering;
using Assman.PreConsolidation;

namespace Assman
{
	public class ResourceConsolidator
	{
		private readonly ContentFilterMap _contentFilterMap;
		private readonly DependencyManager _dependencyManager;
		private readonly IResourceGroupManager _scriptGroups;
		private readonly IResourceGroupManager _styleGroups;
		private readonly IResourceFinder _finder;

		public ResourceConsolidator(ContentFilterMap contentFilterMap, DependencyManager dependencyManager, IResourceGroupManager scriptGroups, IResourceGroupManager styleGroups, IResourceFinder finder)
		{
			_contentFilterMap = contentFilterMap;
			_dependencyManager = dependencyManager;
			_scriptGroups = scriptGroups;
			_styleGroups = styleGroups;
			_finder = finder;
		}

		public ConsolidatedResource ConsolidateGroup(string groupConsolidatedUrl, GroupTemplateContext groupTemplateContext, ResourceMode mode)
		{
			var group = groupTemplateContext.FindGroupOrDefault(_finder, groupConsolidatedUrl, mode);

			if (group == null)
			{
				throw new Exception("No group with consolidatedUrl '" + groupConsolidatedUrl + "' could be found.");
			}

			return ConsolidateGroup(group, mode);
		}

		public ConsolidatedResource ConsolidateGroup(IResourceGroup group, ResourceMode mode)
		{
			Func<IResource, IContentFilter> createContentFilter = resource =>
			{
				var contentFilterFactory = _contentFilterMap.GetFilterFactoryForExtension(resource.FileExtension);
				var settings = new ResourceContentSettings {Minify = group.ShouldMinify(mode)};
				return contentFilterFactory.CreateFilter(settings);
			};

			return group.GetResources()
				.SortByDependencies(_dependencyManager)
				.Consolidate(createContentFilter, group.ResourceType.Separator);
		}

		public PreConsolidationReport ConsolidateAll(Action<ConsolidatedResource, IResourceGroup> handleConsolidatedResource, Action<CompiledResource> handleCompiledIndividualResource, ResourceMode mode)
		{
			return new PreConsolidationReport
			{
				Scripts = ConsolidateAllResources(ResourceType.Script, mode, handleConsolidatedResource, handleCompiledIndividualResource),
				Stylesheets = ConsolidateAllResources(ResourceType.Stylesheet, mode, handleConsolidatedResource, handleCompiledIndividualResource),
				Dependencies = GetAllDependencies(mode).ToList()
			};
		}

		public IEnumerable<CompiledResource> CompileUnconsolidatedResources(ResourceType resourceType, ResourceMode resourceMode, Action<CompiledResource> handleCompiledResource)
		{
			var resources = _finder.FindResources(resourceType);
			var groupManager = GroupManagerFor(resourceType);

			var unconsolidatedResources = (from resource in resources
										  where !groupManager.IsPartOfGroup(resource.VirtualPath)
											&& CanCompileIndividually(resource)
										  select resource).ToList();

			var compiledResources = new List<CompiledResource>();
			foreach (var unconsolidatedResource in unconsolidatedResources)
			{
				var compiledResource = CompileResource(unconsolidatedResource, resourceMode);
				handleCompiledResource(compiledResource);
				compiledResources.Add(compiledResource);
			}

			return compiledResources;
		}

		public CompiledResource CompileResource(IResource resource, ResourceMode resourceMode)
		{
			var contentFilter = _contentFilterMap.GetFilterForExtension(resource.FileExtension, resourceMode);
			var compiledContent = contentFilter.FilterContent(resource.GetContent());

			return new CompiledResource
			{
				Resource = resource,
				Mode = resourceMode,
				CompiledContent = compiledContent
			};
		}

		private bool CanCompileIndividually(IResource resource)
		{
			//currently we only know how to compile file resources.  In order to compile other resources (e.g. embedded resources)
			//we would need to figure out how to generate a unique file path to compile it to
			//this seems like low priority, but will reconsider later
			return resource.VirtualPath.StartsWith("~/");
		}

		private PreConsolidatedResourceReport ConsolidateAllResources(ResourceType resourceType, ResourceMode mode, Action<ConsolidatedResource, IResourceGroup> handleConsolidatedResource, Action<CompiledResource> handleCompiledIndividualResource)
		{
			var groupManager = GroupManagerFor(resourceType);

			return new PreConsolidatedResourceReport
			{
				Groups = ConsolidateGroupsInternal(resourceType, groupManager, mode, handleConsolidatedResource).ToList(),
				SingleResources = CompileUnconsolidatedResources(resourceType, mode, handleCompiledIndividualResource)
					.Select(r => new PreCompiledSingleResource
					{
						OriginalPath = r.Resource.VirtualPath,
						CompiledPath = r.CompiledPath
					}).ToList()
			};
		}

		private IEnumerable<PreConsolidatedResourceGroup> ConsolidateGroupsInternal(ResourceType resourceType,
																				 IResourceGroupManager groupTemplates,
																				 ResourceMode mode,
																				 Action<ConsolidatedResource, IResourceGroup> handleConsolidatedResource)
		{
			if (!groupTemplates.Any())
				return Enumerable.Empty<PreConsolidatedResourceGroup>();

			var allResources = _finder.FindResources(resourceType);

			var preConsolidatedGroups = new List<PreConsolidatedResourceGroup>();
			groupTemplates.EachGroup(allResources, mode, group =>
			{
				var consolidatedResource = ConsolidateGroup(group, mode);
				handleConsolidatedResource(consolidatedResource, group);
				var preConsolidatedGroup = new PreConsolidatedResourceGroup
				{
					ConsolidatedUrl = group.ConsolidatedUrl,
					Resources = consolidatedResource.Resources.Select(resource => resource.VirtualPath).ToList()
				};
				preConsolidatedGroups.Add(preConsolidatedGroup);
			});

			return preConsolidatedGroups;
		}

		private IEnumerable<PreConsolidatedResourceDependencies> GetAllDependencies(ResourceMode mode)
		{
			var allResources = _finder.FindResources(ResourceType.Script)
				.Union(_finder.FindResources(ResourceType.Stylesheet));

			//we must gather the dependencies for the consolidated url's before we gather them for specific url's
			//because the consolidated url's could actually exist on disk if pre-consolidation was run before.
			//In that case, if we did the specific resources first, it would use the IDependencyProvider against the
			//content of the consolidated resource, which is not what we want.  Then that value is cached and prevents us from finding
			//the dependencies for the group.
			var scriptDependenciesByConsolidatedUrl = GetDependenciesForConsolidatedUrls(_scriptGroups, allResources, mode);
			var styleDependenciesByConsolidatedUrl = GetDependenciesForConsolidatedUrls(_styleGroups, allResources, mode);

			var specificResourceDependencies = from resource in allResources
											   let dependencies = _dependencyManager.GetDependencies(resource)
											   where dependencies.Any()
											   select new PreConsolidatedResourceDependencies
											   {
												   ResourcePath = resource.VirtualPath,
												   Dependencies = dependencies.ToList()
											   };

			return
				scriptDependenciesByConsolidatedUrl.Union(styleDependenciesByConsolidatedUrl).Union(
					specificResourceDependencies);
		}

		private IResourceGroupManager GroupManagerFor(ResourceType resourceType)
		{
			if (resourceType == ResourceType.Stylesheet)
				return _styleGroups;
			else
				return _scriptGroups;
		}

		private IEnumerable<PreConsolidatedResourceDependencies> GetDependenciesForConsolidatedUrls(
			IResourceGroupManager groupTemplates, IEnumerable<IResource> allResources, ResourceMode mode)
		{
			var preConsolidatedDependencies = new List<PreConsolidatedResourceDependencies>();

			groupTemplates.EachGroup(allResources, mode, @group =>
			{
				var dependencies = _dependencyManager.GetDependencies(@group.ConsolidatedUrl);
				if (dependencies.Any())
				{
					preConsolidatedDependencies.Add(new PreConsolidatedResourceDependencies
					{
						ResourcePath = @group.ConsolidatedUrl,
						Dependencies = dependencies.ToList()
					});
				}
			});

			return preConsolidatedDependencies;
		}
	}
}