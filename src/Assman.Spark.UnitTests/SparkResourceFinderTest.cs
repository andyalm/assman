using System;
using System.Linq;
using System.Web.Mvc;

using Moq;

using NUnit.Framework;



namespace Assman.Spark
{
	[TestFixture]
	public class SparkResourceFinderTest
	{
		private SparkResourceFinder _resourceFinder;
		private Mock<ISparkResourceContentFetcher> _fetcher;
		private Mock<ISparkJavascriptActionFinder> _actionFinder;

		[SetUp]
		public void Init()
		{
			_fetcher = new Mock<ISparkResourceContentFetcher>();

			_actionFinder = new Mock<ISparkJavascriptActionFinder>();
			
			_resourceFinder = new SparkResourceFinder(new[] { this.GetType().Assembly }, _fetcher.Object, _actionFinder.Object);
		}

		[Test]
		public void WhenResourceTypeIsCss_EmptyResourceCollectionReturned()
		{
			int resourceCount = _resourceFinder.FindResources(ResourceType.Css).Count();

            Assert.That(resourceCount, Is.EqualTo(0));
		}

		[Test]
		public void WhenFindingResources_SparkActionsAreAggregatedAndConvertedToSparkResources()
		{
			_actionFinder.Setup(f => f.FindJavascriptActions(typeof (MyFirstController))).Returns(new[]
			{
				new SparkJavascriptAction
				{
					ActionName = "Action1",
                    ViewName = "View1"
				},
				new SparkJavascriptAction()
				{
					ActionName = "Action2",
					ViewName = "View2"
				}
			});

			_actionFinder.Setup(f => f.FindJavascriptActions(typeof(MySecondController))).Returns(new[]
			{
				new SparkJavascriptAction
				{
					ActionName = "Action3",
                    ViewName = "View3"
				},
				new SparkJavascriptAction()
				{
					ActionName = "Action4",
					ViewName = "View4"
				}
			});

			var resources = _resourceFinder.FindResources(ResourceType.ClientScript).ToList();

			Assert.That(resources.Count, Is.EqualTo(4));
			Assert.That(resources[0].Name, Is.EqualTo("MyFirst/Action1"));
			Assert.That(resources[1].Name, Is.EqualTo("MyFirst/Action2"));
			Assert.That(resources[2].Name, Is.EqualTo("MySecond/Action3"));
			Assert.That(resources[3].Name, Is.EqualTo("MySecond/Action4"));
		}

		private class MyFirstController : Controller
		{
			
		}

		private class MySecondController : Controller
		{
				
		}
	}
}