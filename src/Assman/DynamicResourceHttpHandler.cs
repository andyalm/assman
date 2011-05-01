using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using Assman.Configuration;

namespace Assman
{
	/// <summary>
	/// An <see cref="IHttpHandler"/> that returns consolidated web resources.
	/// </summary>
	public class DynamicResourceHttpHandler : HttpHandlerBase
	{
		private readonly IResourceHandlerFactory _handlerFactory;
		private readonly IDictionary<string, IResourceHandler> _handlerRegistry;
		private readonly ResourceManagementContext _resourceContext;
		private readonly DateTime _minLastModified;
		internal Func<string, string> ToAppRelativePath = path => VirtualPathUtility.ToAppRelative(path);

		public DynamicResourceHttpHandler() : this(ResourceManagementContext.Current, new ResourceHandlerFactory()) {}

		internal DynamicResourceHttpHandler(ResourceManagementContext context, IResourceHandlerFactory handlerFactory)
		{
			_handlerFactory = handlerFactory;
			_resourceContext = context;
			_minLastModified = context.ConfigurationLastModified;
			_handlerRegistry = new Dictionary<string, IResourceHandler>(StringComparer.OrdinalIgnoreCase);
		}

		public override void ProcessRequest(HttpContextBase httpContext)
		{
			string appRelativePath = ToAppRelativePath(httpContext.Request.Path);
			var handler = GetHandler(appRelativePath);
			if (handler == null)
			{
				httpContext.Response.StatusCode = 404;
				return;
			}

			handler.HandleRequest(new HttpRequestContext(httpContext));
		}

		private IResourceHandler GetHandler(string appRelativePath)
		{
			if (_handlerRegistry.ContainsKey(appRelativePath))
			{
				return _handlerRegistry[appRelativePath];
			}

			var templateContext = _resourceContext.FindGroupTemplate(appRelativePath);
			if(templateContext == null)
			{
				return null;
			}
			var handler = _handlerFactory.CreateHandler(appRelativePath, _resourceContext.GetConsolidator(), templateContext);
			handler.MinLastModified = _minLastModified;
			
			_handlerRegistry[appRelativePath] = handler;

			return handler;
		}

		private class HttpRequestContext : IRequestContext
		{
			private readonly HttpContextBase _httpContext;

			public HttpRequestContext(HttpContextBase httpContext)
			{
				_httpContext = httpContext;
				var resourceCacheKey = httpContext.Request.QueryString["c"];
				if (!String.IsNullOrEmpty(resourceCacheKey))
					_httpContext.SetResourceCacheKey(resourceCacheKey);
			}

			public DateTime? IfModifiedSince
			{
				get
				{
					DateTime ifModifiedSince;
					if (DateTime.TryParse(_httpContext.Request.Headers["If-Modified-Since"], out ifModifiedSince))
						return ifModifiedSince;
					else
						return null;
				}
			}

			public int StatusCode
			{
				get { return _httpContext.Response.StatusCode; }
				set { _httpContext.Response.StatusCode = value; }
			}

			public string ContentType
			{
				get { return _httpContext.Response.ContentType; }
				set { _httpContext.Response.ContentType = value; }
			}

			public Stream OutputStream
			{
				get { return _httpContext.Response.OutputStream; }
			}

			public string StatusDescription
			{
				get { return _httpContext.Response.StatusDescription; }
				set { _httpContext.Response.StatusDescription = value; }
			}

			public void SetLastModified(DateTime lastModified)
			{
				_httpContext.Response.AddHeader("Last-Modified", lastModified.ToString("r"));
			}
		}
	}
}
