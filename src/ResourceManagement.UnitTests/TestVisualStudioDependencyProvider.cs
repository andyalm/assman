using AlmWitt.Web.ResourceManagement.TestSupport;

using NUnit.Framework;

using System.Linq;

namespace AlmWitt.Web.ResourceManagement
{
	[TestFixture]
	public class TestVisualStudioDependencyProvider
	{
		private VisualStudioScriptDependencyProvider _provider;

		[SetUp]
		public void SetupContext()
		{
			_provider = new VisualStudioScriptDependencyProvider();
		}

		[Test]
		public void TripleWackCommentReferenceElementWithPathAttributeIsParsedAsDependency()
		{
			var resource = StubResource.WithContent(@"///<reference path=""~/scripts/jquery.js"" />");

			var dependencies = _provider.GetDependencies(resource).ToList();

			dependencies.CountShouldEqual(1);
			dependencies[0].ShouldEqual("~/scripts/jquery.js");
		}

		[Test]
		public void ReferenceElementWithNameAndAssemblyAttributeReturnsEmbeddedResourceVirtualPath()
		{
			var resource = StubResource.WithContent(@"///<reference name=""MyScript.js"" assembly=""MyAssembly"" />");

			var dependencies = _provider.GetDependencies(resource).ToList();

			dependencies.CountShouldEqual(1);
			dependencies[0].ShouldEqual("assembly://MyAssembly/MyScript.js");
		}

		[Test]
		public void ReferenceElementeWithNameButMissingAssemblyAttributeReturnsEmbeddedResourceVirtualPathForSystemWebExtensionsAssembly()
		{
			var resource = StubResource.WithContent(@"///<reference name=""MicrosoftAjax.js"" />");

			var dependencies = _provider.GetDependencies(resource).ToList();

			dependencies.CountShouldEqual(1);
			dependencies[0].ShouldEqual("assembly://System.Web.Extensions/MicrosoftAjax.js");
		}

		[Test]
		public void MultipleReferenceElementsAreRecognized()
		{
			var resource = StubResource.WithContent(@"///<reference path=""~/scripts/script1.js"" />

				///<reference path=""~/scripts/script2.js"" />");

			var dependencies = _provider.GetDependencies(resource).ToList();

			dependencies.CountShouldEqual(2);
			dependencies[0].ShouldEqual("~/scripts/script1.js");
			dependencies[1].ShouldEqual("~/scripts/script2.js");
		}

		//TODO: Handle relative paths
	}
}