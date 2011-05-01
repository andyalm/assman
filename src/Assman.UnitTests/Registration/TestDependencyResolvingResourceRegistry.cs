using Assman.Configuration;
using Assman.TestSupport;

using Moq;

using NUnit.Framework;

namespace Assman.Registration
{
	[TestFixture]
	public class TestDependencyResolvingResourceRegistry
	{
		private DependencyResolvingResourceRegistry _registry;
		private ResourceManagementContext _context;
		private Mock<IResourceRegistry> _inner;
		private Mock<IResourceFinder> _finder;
		private StubDependencyProvider _dependencyProvider;

		[SetUp]
		public void SetupContext()
		{
			_inner = new Mock<IResourceRegistry>();
			_dependencyProvider = new StubDependencyProvider();
			_finder = new Mock<IResourceFinder>();
			_finder.Setup(f => f.FindResource(It.IsAny<string>())).Returns(
				(string virtualPath) => StubResource.WithPath(virtualPath));
			_context = new ResourceManagementContext();
			_context.AddFinder(_finder.Object);
			_context.MapExtensionToDependencyProvider(".js", _dependencyProvider);
			
			_registry = new DependencyResolvingResourceRegistry(_inner.Object, _context);
		}

		[Test]
		public void WhenIncludingPath_ResourcesDependenciesAreIncludedAsWell()
		{
			_dependencyProvider.SetDependencies("~/scripts/myscript.js", "~/scripts/jquery.js");
			
			_registry.IncludePath("~/scripts/myscript.js");

			_inner.Verify(r => r.IncludePath("~/scripts/jquery.js"));
			_inner.Verify(r => r.IncludePath("~/scripts/myscript.js"));
		}
	}
}