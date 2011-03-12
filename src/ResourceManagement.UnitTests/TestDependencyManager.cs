using System.Collections.Generic;

using AlmWitt.Web.ResourceManagement.TestSupport;

using NUnit.Framework;

using System.Linq;

namespace AlmWitt.Web.ResourceManagement
{
	[TestFixture]
	public class TestDependencyManager
	{
		private StubDependencyProvider _dependencyProvider;
		private StubResourceFinder _resourceFinder;
		private InMemoryDependencyCache _dependencyCache;
		private DependencyManager _dependencyManager;

		[SetUp]
		public void SetupContext()
		{
			_dependencyProvider = new StubDependencyProvider();
			_resourceFinder = new StubResourceFinder();
			_dependencyCache = new InMemoryDependencyCache();

			_dependencyManager = new DependencyManager(_resourceFinder, _dependencyCache);
			_dependencyManager.MapProvider(".js", _dependencyProvider);
		}

		[Test]
		public void GetDependenciesReturnsParsedDependencies()
		{
			var myscript = StubResource.WithPath("~/scripts/myscript.js");
			_resourceFinder.AddResource(myscript);

			SetDependencies(myscript, "~/scripts/jquery.js", "~/scripts/common.js");

			var dependencies = _dependencyManager.GetDependencies(myscript.VirtualPath).ToList();
			dependencies[0].ShouldEqual("~/scripts/jquery.js");
			dependencies[1].ShouldEqual("~/scripts/common.js");
		}

		[Test]
		public void GetDependenciesReturnsRecursiveDependenciesInOrder()
		{
			var jquery = StubResource.WithPath("~/scripts/jquery.js");
			var jquerypluginA = StubResource.WithPath("~/scripts/jquery-plugin-a.js");
			var jquerypluginB = StubResource.WithPath("~/scripts/jquery-plugin-b.js");
			var myscript = StubResource.WithPath("~/scripts/myscript.js");

			_resourceFinder.AddResources(jquery, jquerypluginA, jquerypluginB, myscript);

			SetDependencies(jquerypluginB, jquery.VirtualPath, jquerypluginA.VirtualPath);
			SetDependencies(jquerypluginA, jquery.VirtualPath);
			SetDependencies(myscript, jquerypluginB.VirtualPath);

			var dependencies = _dependencyManager.GetDependencies(myscript.VirtualPath).ToList();
			dependencies.CountShouldEqual(3);
			dependencies[0].ShouldEqual(jquery.VirtualPath);
			dependencies[1].ShouldEqual(jquerypluginA.VirtualPath);
			dependencies[2].ShouldEqual(jquerypluginB.VirtualPath);
		}

		[Test]
		public void GetDependenciesReturnsEmptyWhenNoDependencyProviderExistsForFileExtension()
		{
			_dependencyManager.GetDependencies("~/Content/mystyles.css").ShouldBeEmpty();
		}

		[Test]
		public void WhenDependenciesAreCachedByVirtualPath_TheyAreReturnedFromThere()
		{
			var myscript = StubResource.WithPath("~/scripts/myscript.js");
			IEnumerable<string> cachedDependencies = new string[] {"~/scripts/jquery.js"};

			_dependencyCache.StoreDependencies(myscript, cachedDependencies);
			
			var returnedDependencies = _dependencyManager.GetDependencies(myscript.VirtualPath).ToList();
			returnedDependencies.CountShouldEqual(1);
			returnedDependencies[0].ShouldEqual("~/scripts/jquery.js");
		}

		[Test]
		public void WhenDependenciesAreCachedByResource_TheyAreReturnedFromThere()
		{
			var myscript = StubResource.WithPath("~/scripts/myscript.js");
			IEnumerable<string> cachedDependencies = new string[] { "~/scripts/jquery.js" };
			_resourceFinder.AddResource(myscript);
			_dependencyCache.StoreDependenciesByResourceOnly(myscript, cachedDependencies);
			
			var returnedDependencies = _dependencyManager.GetDependencies(myscript.VirtualPath).ToList();
			returnedDependencies.CountShouldEqual(1);
			returnedDependencies[0].ShouldEqual("~/scripts/jquery.js");
		}

		[Test]
		public void DependenciesAreRecursivelyCachedByBothVirtualPathAndResource()
		{
			var jquery = StubResource.WithPath("~/scripts/jquery.js");
			var jquerypluginA = StubResource.WithPath("~/scripts/jquery-plugin-a.js");
			var jquerypluginB = StubResource.WithPath("~/scripts/jquery-plugin-b.js");
			var myscript = StubResource.WithPath("~/scripts/myscript.js");

			_resourceFinder.AddResources(jquery, jquerypluginA, jquerypluginB, myscript);

			SetDependencies(jquerypluginB, jquery.VirtualPath, jquerypluginA.VirtualPath);
			SetDependencies(jquerypluginA, jquery.VirtualPath);
			SetDependencies(myscript, jquerypluginB.VirtualPath);

			_dependencyManager.GetDependencies(myscript.VirtualPath).ToList();
			
			VerifyDependenciesAreCached(myscript.VirtualPath, jquery.VirtualPath, jquerypluginA.VirtualPath, jquerypluginB.VirtualPath);
			VerifyDependenciesAreCached(jquerypluginB.VirtualPath, jquery.VirtualPath, jquerypluginA.VirtualPath);
			VerifyDependenciesAreCached(jquerypluginA.VirtualPath, jquery.VirtualPath);
		}

		private void VerifyDependenciesAreCached(string resourcePath, params string[] expectedDependencies)
		{
			IEnumerable<string> actualDependencies;
			_dependencyCache.TryGetDependencies(resourcePath, out actualDependencies).ShouldBeTrue();
			actualDependencies.CountShouldEqual(expectedDependencies.Length);
			actualDependencies.SequenceEqual(expectedDependencies).ShouldBeTrue();
		}

		private void SetDependencies(IResource resource, params string[] dependencies)
		{
			_dependencyProvider.SetDependencies(resource, dependencies);
		}
	}
}