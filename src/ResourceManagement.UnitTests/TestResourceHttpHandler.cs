using System;
using System.Collections.Generic;
using System.Web;

using AlmWitt.Web.ResourceManagement.Configuration;
using AlmWitt.Web.ResourceManagement.TestSupport;

using Moq;

using NUnit.Framework;

namespace AlmWitt.Web.ResourceManagement
{
	[TestFixture]
	public class TestResourceHttpHandler
	{
		private ResourceHttpHandler _httpHandler;
		private ResourceManagementContext _context;
		private FakeHandlerFactory _handlerFactory;
		private Mock<HttpContextBase> _httpContext;
		private ClientScriptGroupElement _groupElement;
		private const string ConsolidatedPath = "/scripts/consolidated.js";

		[SetUp]
		public void SetupContext()
		{
			_context = ResourceManagementContext.Create();
			_groupElement = new ClientScriptGroupElement();
			_groupElement.ConsolidatedUrl = "~" + ConsolidatedPath;
			_context.ClientScriptGroups.Add(_groupElement);

			_handlerFactory = new FakeHandlerFactory();

			_httpHandler = new ResourceHttpHandler(_context, _handlerFactory);
			_httpHandler.ToAppRelativePath = ToAppRelative;
		}

		[Test]
		public void WhenUrlIsFirstRequested_NewHandlerIsCreatedAndCalled()
		{
			var scriptsPath = ConsolidatedPath + "x";
			ProcessRequest(scriptsPath);

			_handlerFactory.NumberOfCreateCalls.ShouldEqual(1);
			_handlerFactory.HandlersCreated[0].Verify(h => h.HandleRequest(It.IsAny<IRequestContext>()));
		}

		[Test]
		public void WhenUrlIsRequestedAgain_HandlerIsReusedAndCalledAgain()
		{
			var scriptsPath = ConsolidatedPath + "x";
			ProcessRequest(scriptsPath);
			ProcessRequest(scriptsPath);

			_handlerFactory.NumberOfCreateCalls.ShouldEqual(1);
			_handlerFactory.HandlersCreated[0].Verify(h => h.HandleRequest(It.IsAny<IRequestContext>()), Times.Exactly(2));
		}

		[Test]
		public void WhenUrlDoesNotMatchAnyGroupTemplates_NoHandlerIsCreatedAnd404IsReturned()
		{
			ProcessRequest("/bogus.jsx");

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
			httpContext.SetupProperty(c => c.Response.StatusCode);

			return httpContext;
		}

		private class FakeHandlerFactory : IResourceHandlerFactory
		{
			private readonly List<Mock<IResourceHandler>> _handlersCreated = new List<Mock<IResourceHandler>>();

			public List<Mock<IResourceHandler>> HandlersCreated
			{
				get { return _handlersCreated; }
			}

			public int NumberOfCreateCalls
			{
				get { return _handlersCreated.Count; }
			}

			public IResourceHandler CreateHandler(string path, ResourceManagementContext configContext, GroupTemplateContext groupTemplateContext)
			{
				var handler = new Mock<IResourceHandler>();
				_handlersCreated.Add(handler);

				return handler.Object;
			}
		}
	}
}