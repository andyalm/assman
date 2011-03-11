using System;
using System.Linq;

using AlmWitt.Web.ResourceManagement.Configuration;
using AlmWitt.Web.ResourceManagement.TestSupport;
using AlmWitt.Web.ResourceManagement.TestObjects;

using Moq;

using NUnit.Framework;

namespace AlmWitt.Web.ResourceManagement
{
	[TestFixture]
	public class TestResourceManagementContext
	{
		[Test]
		public void ConsolidateGroupExcludesResourcesMatchingGivenExcludeFilter()
		{
			var stubFinder = new StubResourceFinder();
			stubFinder.AddResource("~/file1.js", "");
			stubFinder.AddResource("~/file2.js", "");
			stubFinder.AddResource("~/file3.js", "");
			
			var group = new ResourceGroup("~/consolidated.js", stubFinder.Resources);
			var groupTemplate = new StubResourceGroupTemplate(group);
			groupTemplate.ResourceType = ResourceType.ClientScript;
			
			var context = new ResourceManagementContext();
			context.AddFinder(stubFinder);
			var consolidatedResource = context.ConsolidateGroup(group.ConsolidatedUrl,
			                                                           new GroupTemplateContext(groupTemplate)
			                                                           {
			                                                           	ExcludeFilter = ToFilter(r => r.VirtualPath.Contains("file2"))
			                                                           }, ResourceMode.Debug);
			
			consolidatedResource.ShouldNotBeNull();
			consolidatedResource.Resources.Count().ShouldEqual(2);
		}

		[Test]
		public void ConsolidateGroupSortsResourcesByDependencies()
		{
			var group = new ResourceGroup("~/consolidated.js", new IResource[]
			{
				StubResource.WithPath("~/dependency-leaf.js"),
				StubResource.WithPath("~/dependency-root.js"),
				StubResource.WithPath("~/dependency-middle.js")
			});

			var groupTemplate = new StubResourceGroupTemplate(group);
			groupTemplate.ResourceType = ResourceType.ClientScript;

			var dependencyProvider = new StubDependencyProvider();
			dependencyProvider.SetDependencies("~/dependency-leaf", "~/dependency-middle.js");
			dependencyProvider.SetDependencies("~/dependency-middle.js", "~/dependency-root.js");
			
			var context = new ResourceManagementContext();
			context.MapExtensionToDependencyProvider(".js", dependencyProvider);

			var consolidatedResource = context.ConsolidateGroup(group, new GroupTemplateContext(groupTemplate), ResourceMode.Debug);
			var resources = consolidatedResource.Resources.ToList();
			resources[0].VirtualPath.ShouldEqual("~/dependency-root.js");
			resources[1].VirtualPath.ShouldEqual("~/dependency-middle.js");
			resources[2].VirtualPath.ShouldEqual("~/dependency-leaf.js");
		}

		//TODO: Write tests for refactored GetResourceUrl

		private IResourceFilter ToFilter(Predicate<IResource> predicate)
		{
			return ResourceFilters.Predicate(predicate);
		}
	}
}