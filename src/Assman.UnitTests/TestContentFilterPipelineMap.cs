using System;

using Assman.Configuration;
using Assman.ContentFiltering;
using Assman.TestSupport;

using NUnit.Framework;

namespace Assman
{
	[TestFixture]
	public class TestContentFilterPipelineMap
	{
		private ContentFilterPipelineMap _pipelineMap;

		[SetUp]
		public void SetupContext()
		{
			_pipelineMap = new ContentFilterPipelineMap();
		}

		[Test]
		public void CanMapFileExtensionThatBeginsWithADot()
		{
		    var pipeline = new ContentFilterPipeline();
			_pipelineMap.MapExtension(".js", pipeline);

			var returnedFilter = _pipelineMap.GetPipelineForExtension(".js");

			returnedFilter.ShouldBeSameAs(pipeline);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void ThrowsWhenTryingToMapExtensionWithoutLeadingDot()
		{
		    var pipeline = new ContentFilterPipeline();
			_pipelineMap.MapExtension("js", pipeline);
		}

		[Test]
		public void WhenGettingFilterForUnmappedExtension_EmptyPipelineIsReturned()
		{
			var factory = _pipelineMap.GetPipelineForExtension(".bogus");

			factory.ShouldBeInstanceOf<ContentFilterPipeline>();
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void WhenGettingPipelineForExtensionWithoutLeadingDot_ExceptionIsThrown()
		{
			_pipelineMap.GetPipelineForExtension("bogus");
		}
	}
}