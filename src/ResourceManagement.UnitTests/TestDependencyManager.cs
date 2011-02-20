using AlmWitt.Web.ResourceManagement.TestSupport;

using Moq;

using NUnit.Framework;

using System.Linq;

namespace AlmWitt.Web.ResourceManagement
{
	[TestFixture]
	public class TestDependencyManager
	{
		private Mock<IDependencyProvider> _dependencyProvider;
		private StubResourceFinder _resourceFinder;
		private DependencyManager _dependencyManager;

		[SetUp]
		public void SetupContext()
		{
			_dependencyProvider = new Mock<IDependencyProvider>();
			_dependencyProvider.Setup(p => p.GetDependencies(It.IsAny<IResource>())).Returns(new string[0]);
			_resourceFinder = new StubResourceFinder();

			_dependencyManager = new DependencyManager(_resourceFinder);
			_dependencyManager.MapProvider(".js", _dependencyProvider.Object);
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

		private void SetDependencies(IResource resource, params string[] dependencies)
		{
			_dependencyProvider.Setup(p => p.GetDependencies(resource)).Returns(dependencies);
		}
	}
}