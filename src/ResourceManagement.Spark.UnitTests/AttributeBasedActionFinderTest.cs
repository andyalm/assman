using System;
using System.Linq;
using System.Web.Mvc;

using AlmWitt.Web.ResourceManagement.Spark.TestSupport;

using NUnit.Framework;

namespace AlmWitt.Web.ResourceManagement.Spark
{
	[TestFixture]
	public class AttributeBasedActionFinderTest
	{
		private AttributeBasedActionFinder _finder;

		[SetUp]
		public void Init()
		{
			_finder = new AttributeBasedActionFinder();
		}
		
		[Test]
		public void WhenControllerContainsSparkScriptActions_TheyAreFound()
		{
			var actions = _finder.FindJavascriptActions(typeof(MyControllerWithSparkActions)).ToList();

			Assert.That(actions.Count, Is.EqualTo(2));
            
			actions.ShouldContain(a => a.ActionName == "ActionWithAttribute1" && a.ViewName == "MyView");
			actions.ShouldContain(a => a.ActionName == "ActionWithAttribute2" && a.ViewName == "MyView" && a.MasterName == "MyMaster");
		}

		[Test]
		public void WhenControllerContainsNoSparkScriptActions_EmptyEnumerableIsReturned()
		{
			_finder.FindJavascriptActions(typeof(MyControllerWithNoSparkActions)).Count().ShouldEqual(0);
		}

		[Test]
		public void WhenAttributeIsOnActionWithNoViewNameSpecified_ActionNameIsUsed()
		{
			var actions = _finder.FindJavascriptActions(typeof (MyControllerWithSparkActionNoViewName));

			actions.ShouldContain(a => a.ActionName == "ActionWithAttribute" && a.ViewName == "ActionWithAttribute");
		}

		private class MyControllerWithSparkActionNoViewName : Controller
		{
			[ConsolidateSparkScript()]
			public ActionResult ActionWithAttribute()
			{
				return null;
			}
		}

		private class MyControllerWithSparkActions : Controller
		{
			public ActionResult ActionWithoutAttribute()
			{
				return null;
			}

			[ConsolidateSparkScript(ViewName = "MyView")]
			public ActionResult ActionWithAttribute1()
			{
				return null;
			}

			[ConsolidateSparkScript(ViewName = "MyView", MasterName = "MyMaster")]
			public ActionResult ActionWithAttribute2()
			{
				return null;
			}
		}

		private class MyControllerWithNoSparkActions : Controller
		{
			public ActionResult ActionWithoutAttribute()
			{
				return null;
			}
		}
	}
}