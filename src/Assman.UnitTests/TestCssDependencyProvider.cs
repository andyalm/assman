using System.Linq;

using Assman.TestSupport;

using NUnit.Framework;

namespace Assman
{
	public class TestCssDependencyProvider
	{
		private CssDependencyProvider _provider;

		[SetUp]
		public void SetupContext()
		{
			_provider = new CssDependencyProvider();
		}

		[Test]
		public void DependencyAttributeWithAbsoluteUrlIsParsedAsDependency()
		{
			var resource = StubResource.WithContent(@"/* dependency:url(/styles/mystylesheet.css); */");

			var dependencies = _provider.GetDependencies(resource).ToList();

			dependencies.CountShouldEqual(1);
			dependencies[0].ShouldEqual("~/styles/mystylesheet.css");
		}

		[Test]
		public void DependencyAttributeWithAppRelativeUrlIsParsedAsDependency()
		{
			var resource = StubResource.WithContent(@"/* dependency:url(~/styles/mystylesheet.css); */");

			var dependencies = _provider.GetDependencies(resource).ToList();

			dependencies.CountShouldEqual(1);
			dependencies[0].ShouldEqual("~/styles/mystylesheet.css");
		}

		[Test]
		public void DependencyAttributeWithoutSemicolonAtTheEndIsStillParsed()
		{
			var resource = StubResource.WithContent(@"/* dependency:url(~/styles/mystylesheet.css) */");

			var dependencies = _provider.GetDependencies(resource).ToList();

			dependencies.CountShouldEqual(1);
			dependencies[0].ShouldEqual("~/styles/mystylesheet.css");
		}

		[Test]
		public void MultipleDependenciesAreRecognized()
		{
			var resource = StubResource.WithContent(@"/* dependency:url(~/styles/mystylesheet.css);

				dependency:url(~/styles/myotherstylesheet.css);*/");

			var dependencies = _provider.GetDependencies(resource).ToList();

			dependencies.CountShouldEqual(2);
			dependencies[0].ShouldEqual("~/styles/mystylesheet.css");
			dependencies[1].ShouldEqual("~/styles/myotherstylesheet.css");
		}
	}
}