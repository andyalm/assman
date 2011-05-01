using System;

using Assman.Configuration;
using Assman.ContentFiltering;
using Assman.TestSupport;

using Moq;

using NUnit.Framework;

namespace Assman
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