using System;
using System.Collections.Generic;
using System.Linq;

using Assman.TestSupport;

using NUnit.Framework;

namespace Assman
{
    [TestFixture]
    public class TestResourceCompiler
    {
        private ResourceTestContext _context;
        private ResourceCompiler _compiler;
        
        [SetUp]
        public void SetupContext()
        {
            _context = new ResourceTestContext(ResourceMode.Release);
            _compiler = _context.GetConsolidator();
        }
        
        [Test]
        public void WhenPreConsolidatedReportIsGenerated_ConsolidatedUrlsDependenciesAreIncludedInReport()
        {
            var coreGroup = _context.CreateGroup("~/scripts/consolidated/core.js");
            var sharedGroup = _context.CreateGroup("~/scripts/consolidated/shared.js");
            var homeGroup = _context.CreateGroup("~/scripts/consolidated/home.js");

            var jquery = _context.CreateResource("~/scripts/jquery.js")
                .InGroup(coreGroup).Resource;
            var site = _context.CreateResource("~/scripts/site.js")
                .WithDependencies(jquery)
                .InGroup(coreGroup).Resource;
            var mycomponent = _context.CreateResource("~/scripts/mycomponent.js")
                .InGroup(sharedGroup)
                .WithDependencies(site).Resource;
            var myothercomponent = _context.CreateResource("~/scripts/myothercomponent.js")
                .InGroup(sharedGroup)
                .WithDependencies(site).Resource;
            var homeIndex = _context.CreateResource("~/Views/Home/Index.js")
                .InGroup(homeGroup)
                .WithDependencies(jquery, mycomponent).Resource;
            var homePartial = _context.CreateResource("~/Views/Home/_MyPartial.js")
                .InGroup(homeGroup)
                .WithDependencies(jquery, site).Resource;
            var accountIndex = _context.CreateResource("~/Views/Account/Index.js")
                .WithDependencies(myothercomponent).Resource;

            var preConsolidatedReport = _compiler.CompileAll(resource => { });
            var homeGroupDepends = preConsolidatedReport.Dependencies.ShouldContain(d => d.ResourcePath == homeGroup.ConsolidatedUrl);
            
            homeGroupDepends.Dependencies.CountShouldEqual(3);
            homeGroupDepends.Dependencies[0].ShouldEqual(jquery.VirtualPath);
            homeGroupDepends.Dependencies[1].ShouldEqual(site.VirtualPath);
            homeGroupDepends.Dependencies[2].ShouldEqual(mycomponent.VirtualPath);
        }

        [Test]
        public void WhenPreConsolidatedReportIsGenerated_UnconsolidatedResourcesAreIncluded()
        {
            _context.CreateGroup("~/group1.js", "~/file1.js", "~/file2.js");
            _context.CreateGroup("~/group2.js", "~/file3.js", "~/file4.js");

            _context.CreateResource("~/file5.js");
            _context.CreateResource("~/file6.js");

            var report = _compiler.CompileAll(r => {});

            report.Scripts.SingleResources.CountShouldEqual(2);
            report.Scripts.SingleResources[0].OriginalPath.ShouldEqual("~/file5.js");
            report.Scripts.SingleResources[0].CompiledPath.ShouldEqual("~/file5.compiled.js");
            report.Scripts.SingleResources[1].OriginalPath.ShouldEqual("~/file6.js");
            report.Scripts.SingleResources[1].CompiledPath.ShouldEqual("~/file6.compiled.js");
        }

        [Test]
        public void WhenPreConsolidatedReportIsGenerated_FilesThatAlreadyHaveMinifiedEquivilentsAreNotCompiled()
        {
            _context.CreateResource("~/file.js");
            _context.CreateResource("~/file.min.js");
            _context.CreateResource("~/ms-file.debug.js");
            _context.CreateResource("~/ms-file.js");

            var compiledResources = new List<ICompiledResource>();
            var report = _compiler.CompileAll(compiledResources.Add);

            report.Scripts.SingleResources.CountShouldEqual(0);
            compiledResources.CountShouldEqual(0);
        }

        [Test]
        public void ConsolidateGroupExcludesResourcesMatchingGivenExcludeFilter()
        {
            var group = _context.CreateGroup("~/consolidated.js");
            
            _context.CreateResource("~/file1.js")
                .InGroup(group);
            _context.CreateResource("~/file2.js")
                .InGroup(group);
            _context.CreateResource("~/file3.js")
                .InGroup(group);

            var groupTemplate = new StubResourceGroupTemplate(group);
            groupTemplate.ResourceType = ResourceType.Script;

            var excludeFilter = ToFilter(r => r.VirtualPath.Contains("file2"));
            var consolidatedResource = _compiler.CompileGroup(group.ConsolidatedUrl, groupTemplate.WithContext(excludeFilter));

            consolidatedResource.ShouldNotBeNull();
            consolidatedResource.Resources.Count().ShouldEqual(2);
        }

        [Test]
        public void ConsolidateGroupSortsResourcesByDependencies()
        {
            var dependencyLeaf1 = _context.CreateResource("~/dependency-leaf1.js").Resource;
            var dependencyLeaf2 = _context.CreateResource("~/dependency-leaf2.js").Resource;
            var dependencyRoot3 = _context.CreateResource("~/dependency-root3.js").Resource;
            var dependencyRoot1 = StubResource.WithPath("~/dependency-root1.js");
            var dependencyRoot1Minified = _context.CreateResource("~/dependency-root1.min.js").Resource;
            var dependencyBranch1 = _context.CreateResource("~/dependency-branch1.js").Resource;
            var dependencyLeaf5 = _context.CreateResource("~/dependency-leaf5.js").Resource;
            var dependencyRoot2 = _context.CreateResource("~/dependency-root2.js").Resource;
            var dependencyBranch2 = _context.CreateResource("~/dependency-branch2.js").Resource;
            var dependencyLeaf4 = _context.CreateResource("~/dependency-leaf4.js").Resource;
            var dependencyLeaf3 = _context.CreateResource("~/dependency-leaf3.js").Resource;
            
            
            var group = new ResourceGroup("~/consolidated.js", new IResource[]
            {
                dependencyLeaf1,
                dependencyLeaf2,
                dependencyRoot1.ExternallyCompiledWith(dependencyRoot1Minified, ResourceMode.Release),
                dependencyRoot2,
                dependencyBranch1,
                dependencyLeaf3,
                dependencyRoot3,
                dependencyBranch2,
                dependencyLeaf4,
                dependencyLeaf5
            });

            var groupTemplate = new StubResourceGroupTemplate(group);
            groupTemplate.ResourceType = ResourceType.Script;

            _context.AddGlobalScriptDependencies(dependencyRoot1, dependencyRoot2, dependencyRoot3);
            _context.DependencyProvider.SetDependencies(dependencyBranch2, dependencyBranch1.VirtualPath);
            _context.DependencyProvider.SetDependencies(dependencyLeaf1, dependencyBranch1.VirtualPath);
            _context.DependencyProvider.SetDependencies(dependencyLeaf2, dependencyBranch1.VirtualPath);
            _context.DependencyProvider.SetDependencies(dependencyLeaf3, dependencyBranch2.VirtualPath);
            _context.DependencyProvider.SetDependencies(dependencyLeaf4, dependencyBranch2.VirtualPath);
            _context.DependencyProvider.SetDependencies(dependencyLeaf5, dependencyBranch1.VirtualPath, dependencyBranch2.VirtualPath);

            var consolidatedResource = _compiler.CompileGroup(group);
            var resources = consolidatedResource.Resources.ToList();
            resources[0].VirtualPath.ShouldEqual(dependencyRoot1.VirtualPath);
            resources[1].VirtualPath.ShouldEqual(dependencyRoot2.VirtualPath);
            resources[2].VirtualPath.ShouldEqual(dependencyRoot3.VirtualPath);
            resources[3].VirtualPath.ShouldEqual(dependencyBranch1.VirtualPath);
            resources[4].VirtualPath.ShouldEqual(dependencyLeaf1.VirtualPath);
            resources[5].VirtualPath.ShouldEqual(dependencyLeaf2.VirtualPath);
            resources[6].VirtualPath.ShouldEqual(dependencyBranch2.VirtualPath);
            resources[7].VirtualPath.ShouldEqual(dependencyLeaf3.VirtualPath);
            resources[8].VirtualPath.ShouldEqual(dependencyLeaf4.VirtualPath);
            resources[9].VirtualPath.ShouldEqual(dependencyLeaf5.VirtualPath);
        }

        [Test]
        public void CompileUnconsolidatedResourcesOnlyCompilesUnconsolidatedResources()
        {
            var group = _context.CreateGroup("~/consolidated.js");

            _context.CreateResource("~/file1.js")
                .InGroup(group);
            _context.CreateResource("~/file2.js")
                .InGroup(group);
            _context.CreateResource("~/file3.js")
                .InGroup(group);

            var unconsolidatedResource1 = _context.CreateResource("~/unconsolidated1.js").Resource;
            var unconsolidatedResource2 = _context.CreateResource("~/unconsolidated2.js").Resource;

            var unconsolidatedResourceCompilations = _compiler.CompileUnconsolidatedResources(ResourceType.Script, r => {}).ToList();

            unconsolidatedResourceCompilations.CountShouldEqual(2);
            unconsolidatedResourceCompilations[0].Resources.Single().VirtualPath.ShouldEqual(unconsolidatedResource1.VirtualPath);
            unconsolidatedResourceCompilations[1].Resources.Single().VirtualPath.ShouldEqual(unconsolidatedResource2.VirtualPath);
        }

        [Test]
        public void CompileUnconsolidatedResourcesSkipsResourcesThatWereCompiledByAnExternalProcess()
        {
            var group = _context.CreateGroup("~/consolidated.js");

            _context.CreateResource("~/file1.js")
                .InGroup(group);
            _context.CreateResource("~/file2.js")
                .InGroup(group);
            _context.CreateResource("~/file3.js")
                .InGroup(group);

            
            var unconsolidatedResource2 = _context.CreateResource("~/unconsolidated2.js").Resource;
            var unconsolidatedResource1 = _context.CreateResource("~/unconsolidated1.js").Resource;
            var unconsolidatedMinifiedResource1 = _context.CreateResource("~/unconsolidated1.min.js").Resource;

            var unconsolidatedResourceCompilations = _compiler.CompileUnconsolidatedResources(ResourceType.Script, r => { }).ToList();

            unconsolidatedResourceCompilations.CountShouldEqual(1);
            unconsolidatedResourceCompilations[0].Resources.Single().VirtualPath.ShouldEqual(unconsolidatedResource2.VirtualPath);
        }

        private IResourceFilter ToFilter(Predicate<IResource> predicate)
        {
            return ResourceFilters.Predicate(predicate);
        }
    }
}