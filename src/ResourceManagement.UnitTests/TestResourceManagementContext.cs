using System;
using System.Linq;

using AlmWitt.Web.ResourceManagement.Configuration;
using AlmWitt.Web.ResourceManagement.TestSupport;
using AlmWitt.Web.ResourceManagement.TestObjects;

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
			var consolidatedResource = context.ConsolidatedGroup(group.ConsolidatedUrl,
			                                                           new GroupTemplateContext(groupTemplate)
			                                                           {
			                                                           	ExcludeFilter = ToFilter(r => r.VirtualPath.Contains("file2"))
			                                                           });
			
			consolidatedResource.ShouldNotBeNull();
			consolidatedResource.Resources.Count().ShouldEqual(2);
		}

		private IResourceFilter ToFilter(Predicate<IResource> predicate)
		{
			return ResourceFilters.Predicate(predicate);
		}
	}
}