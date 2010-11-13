using System;

using AlmWitt.Web.ResourceManagement.Configuration;
using AlmWitt.Web.ResourceManagement.ContentFiltering;
using AlmWitt.Web.ResourceManagement.TestSupport;

using Moq;

using NUnit.Framework;

namespace AlmWitt.Web.ResourceManagement
{
	[TestFixture]
	public class TestContentFilterMap
	{
		private ContentFilterMap _map;

		[SetUp]
		public void SetupContext()
		{
			_map = new ContentFilterMap();
		}

		[Test]
		public void CanMapFileExtensionThatBeginsWithADot()
		{
			var factory = new Mock<IContentFilterFactory>().Object;
			_map.MapExtension(".js", factory);

			var returnedFilter = _map.GetFilterFactoryForExtension(".js");

			returnedFilter.ShouldBeSameAs(factory);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void ThrowsWhenTryingToMapExtensionWithoutLeadingDot()
		{
			var factory = new Mock<IContentFilterFactory>().Object;
			_map.MapExtension("js", factory);
		}

		[Test]
		public void WhenGettingFilterForUnmappedExtension_NullFilterIsReturned()
		{
			var factory = _map.GetFilterFactoryForExtension(".bogus");

			factory.ShouldBeInstanceOf<NullContentFilterFactory>();
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void WhenGettingFilterForExtensionWithoutLeadingDot_ExceptionIsThrown()
		{
			_map.GetFilterFactoryForExtension("bogus");
		}
	}
}