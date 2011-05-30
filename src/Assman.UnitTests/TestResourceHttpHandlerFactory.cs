using System;
using System.Web;
using System.Web.Configuration;

using Assman.Configuration;
using Assman.Handlers;
using Assman.TestSupport;

using Moq;

using NUnit.Framework;

namespace Assman
{
	[TestFixture]
	public class TestResourceHttpHandlerFactory
	{
		private ResourceHttpHandlerFactory _handlerFactory;
		private AssmanContext _context;
		private Mock<IConfigLoader> _configLoader;
		private ScriptGroupElement _groupElement;
		private const string ConsolidatedPath = "/scripts/consolidated.js";
		private const string GET = "GET";

		[SetUp]
		public void SetupContext()
		{
			_context = AssmanContext.Create();
			_groupElement = new ScriptGroupElement();
			_groupElement.ConsolidatedUrl = "~" + ConsolidatedPath;
			_context.ScriptGroups.Add(_groupElement);

			_configLoader = new Mock<IConfigLoader>();
		    _configLoader.Setup(l => l.GetSection<CompilationSection>(It.IsAny<string>())).Returns(new CompilationSection());

			_handlerFactory = new ResourceHttpHandlerFactory(_context, _configLoader.Object);
		}

		[Test]
		public void WhenHandlerForAUrlIsRequestedMoreThanOnce_ItReturnsTheSameInstance()
		{
			var handler = GetHandler(ConsolidatedPath);
			var handler2 = GetHandler(ConsolidatedPath);

			handler.ShouldBeSameAs(handler2);
		}

		[Test]
		public void WhenUrlIsNotAConsolidatedUrl_UnconsolidatedHandlerIsReturned()
		{
			var handler = GetHandler("/bogus.js");

			handler.ShouldBeInstanceOf<DynamicallyCompiledIndividualResourceHandler>();
		}

		[Test]
		public void WhenUrlIsAConsolidatedUrl_ConsolidatedHandlerIsReturned()
		{
			var handler = GetHandler(ConsolidatedPath);

			handler.ShouldBeInstanceOf<DynamicallyConsolidatedResourceHandler>();
		}

		private IHttpHandler GetHandler(string url)
		{
			return _handlerFactory.GetHandler(null, GET, url, ToPhysicalPath(url));
		}

		private string ToPhysicalPath(string path)
		{
			return "c:\\inetpub\\wwwroot\\MyWebsite" + path.Replace("/", "\\");
		}
	}
}