using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;

using Assman.Configuration;
using Assman.Handlers;
using Assman.TestSupport;

using Moq;

using NUnit.Framework;

namespace Assman
{
	[TestFixture]
	public class TestDynamicResourceHttpHandler
	{
		private ConsolidatedResourceHandler _httpHandler;
		private AssmanContext _context;
		private FakeHandlerFactory _handlerFactory;
		private Mock<HttpContextBase> _httpContext;
		private ScriptGroupElement _groupElement;
		private const string ConsolidatedPath = "/scripts/consolidated.js";

		[SetUp]
		public void SetupContext()
		{
			_context = AssmanContext.Create();
			_groupElement = new ScriptGroupElement();
			_groupElement.ConsolidatedUrl = "~" + ConsolidatedPath;
			_context.ScriptGroups.Add(_groupElement);

			_handlerFactory = new FakeHandlerFactory();

			_httpHandler = new ConsolidatedResourceHandler(_context, _handlerFactory);
			_httpHandler.ToAppRelativePath = ToAppRelative;
		}

		[Test]
		public void WhenUrlIsFirstRequested_NewHandlerIsCreatedAndCalled()
		{
			var scriptsPath = ConsolidatedPath;
			ProcessRequest(scriptsPath);

			_handlerFactory.NumberOfCreateCalls.ShouldEqual(1);
			_handlerFactory.HandlersCreated[0].Verify(h => h.HandleRequest(It.IsAny<IRequestContext>()));
		}

		[Test]
		public void WhenUrlIsRequestedAgain_HandlerIsReusedAndCalledAgain()
		{
			var scriptsPath = ConsolidatedPath;
			ProcessRequest(scriptsPath);
			ProcessRequest(scriptsPath);

			_handlerFactory.NumberOfCreateCalls.ShouldEqual(1);
			_handlerFactory.HandlersCreated[0].Verify(h => h.HandleRequest(It.IsAny<IRequestContext>()), Times.Exactly(2));
		}

		[Test]
		public void WhenUrlDoesNotMatchAnyGroupTemplates_NoHandlerIsCreatedAnd404IsReturned()
		{
			ProcessRequest("/bogus.js");

			_handlerFactory.NumberOfCreateCalls.ShouldEqual(0);
			_httpContext.Object.Response.StatusCode.ShouldEqual(404);
		}

		private string ToAppRelative(string path)
		{
			return "~" + path;
		}

		private void ProcessRequest(string path)
		{
			_httpContext = CreateHttpContext(path);
			_httpHandler.ProcessRequest(_httpContext.Object);
		}

		private Mock<HttpContextBase> CreateHttpContext(string path)
		{
			var httpContext = new Mock<HttpContextBase>();
			httpContext.Setup(c => c.Request.Path).Returns(path);
			httpContext.Setup(c => c.Request.QueryString).Returns(new NameValueCollection());
			httpContext.SetupProperty(c => c.Response.StatusCode);

			return httpContext;
		}
	}
}