using System;

using AlmWitt.Web.ResourceManagement.Spark.UnitTests.TestSupport;

using Moq;

using NUnit.Framework;

namespace AlmWitt.Web.ResourceManagement.Spark.UnitTests
{
	[TestFixture]
	public class SparkResourceTest
	{
		private SparkResource _resource;
		private Mock<ISparkResourceContentFetcher> _contentFetcher;

		[SetUp]
		public void Init()
		{
			_contentFetcher = new Mock<ISparkResourceContentFetcher>();

			var sparkJavascriptAction = new SparkJavascriptAction { ActionName = "MyAction", ViewName = "MyView", MasterName = "MyMaster"};
			
			_resource = new SparkResource("MyController", sparkJavascriptAction, _contentFetcher.Object);
		}

		[Test]
		public void NameIsControllerSlashAction()
		{
			_resource.Name.ShouldEqual("MyController/MyAction");
		}

		[Test]
		public void VirtualPathIsSparkJsSchemeUriWithControllerSlashAction()
		{
			_resource.VirtualPath.ShouldEqual("sparkjs://MyController/MyAction");
		}

		[Test]
		public void ToStringEqualsVirtualPath()
		{
			_resource.ToString().ShouldEqual(_resource.VirtualPath);
		}

		[Test]
		public void GetContentRetrievesContentFromFetcher()
		{
			_resource.GetContent();

			_contentFetcher.Verify(f => f.GetContent("MyController", "MyView", "MyMaster"));
		}
	}
}