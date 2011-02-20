using AlmWitt.Web.ResourceManagement.Configuration;

using Moq;

using NUnit.Framework;

namespace AlmWitt.Web.ResourceManagement.Registration
{
	[TestFixture]
	public class TestDependencyResolvingResourceRegistry
	{
		private DependencyResolvingResourceRegistry _registry;
		private ResourceManagementContext _context;
		private Mock<IResourceRegistry> _inner;

		[SetUp]
		public void SetupContext()
		{
			_inner = new Mock<IResourceRegistry>();
			_context = new ResourceManagementContext();
			
			_registry = new DependencyResolvingResourceRegistry(_inner.Object, _context);
		}

		[Test]
		public void WhenIncludingPath_ResourcesDependenciesAreIncludedAsWell()
		{
			_registry.IncludePath("~/scripts/myscript.js");

			_inner.Verify(r => r.IncludePath("~/scripts/jquery.js"));
			_inner.Verify(r => r.IncludePath("~/scripts/myscript.js"));
		}
	}
}