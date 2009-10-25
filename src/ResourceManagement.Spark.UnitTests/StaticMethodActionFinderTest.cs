using System;
using System.Web.Mvc;

using AlmWitt.Web.ResourceManagement.Spark.UnitTests.TestSupport;

using NUnit.Framework;

using Spark.Web.Mvc;

using System.Linq;

namespace AlmWitt.Web.ResourceManagement.Spark.UnitTests
{
	[TestFixture]
	public class StaticMethodActionFinderTest
	{
		private StaticMethodActionFinder _finder;

		[SetUp]
		public void Init()
		{
			_finder = new StaticMethodActionFinder();
		}

		[Test]
		public void AllStaticMethodsWithJavascriptViewResultReturnTypeAreFound()
		{
			var actions = _finder.FindJavascriptActions(typeof(MyControllerWithActions)).ToList();

			actions.Count.ShouldEqual(2);

			actions.ShouldContain(a => a.ActionName == "SparkAction1" && a.ViewName == "MyView1");
			actions.ShouldContain(a => a.ActionName == "SparkAction2" && a.ViewName == "MyView2" && a.MasterName == "Master2");
		}

		private class MyControllerWithActions
		{
			public static string NotAnAction()
			{
				return String.Empty;
			}

			public static ActionResult NotASparkJavascriptAction()
			{
				return new ViewResult();
			}

			public static JavascriptViewResult SparkAction1()
			{
				return new JavascriptViewResult { ViewName = "MyView1" };
			}

			public static JavascriptViewResult SparkAction2()
			{
				return new JavascriptViewResult { MasterName = "Master2", ViewName = "MyView2" };
			}
		}
	}
}