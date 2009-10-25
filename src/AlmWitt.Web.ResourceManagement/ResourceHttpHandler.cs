using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using AlmWitt.Web.ResourceManagement.Configuration;

namespace AlmWitt.Web.ResourceManagement
{
	/// <summary>
	/// An <see cref="IHttpHandler"/> that returns consolidated web resources.
	/// </summary>
	public class ResourceHttpHandler : IHttpHandler
	{
		private static IDictionary<string, ResourceHandler> _handlerRegistry;

		#region Static Init

		static ResourceHttpHandler()
		{
			Init();
		}

		private static void Init()
		{
			string basePath = HttpContext.Current.Server.MapPath("~");
            IResourceFinder finder = ResourceFinderFactory.GetInstance(basePath);
			ResourceManagementConfiguration config = ResourceManagementConfiguration.Current;
		    finder = config.AddCustomFinders(finder);
			DateTime minLastModified = ConfigurationHelper.GetLastModified(config);

			_handlerRegistry = new Dictionary<string,ResourceHandler>();
			config.ClientScripts.ProcessEach(delegate(ClientScriptGroupElement groupElement, IResourceFilter excludeFilter)
			{
				ClientScriptResourceHandler scriptHandler = new ClientScriptResourceHandler(finder, GetCollector(groupElement), excludeFilter);
				RegisterHandler(groupElement.ConsolidatedUrl, scriptHandler, minLastModified);
			});
			
			config.CssFiles.ProcessEach(delegate(CssGroupElement groupElement, IResourceFilter excludeFilter)
			{
				CssResourceHandler cssHandler = new CssResourceHandler(finder, GetCollector(groupElement), excludeFilter);
				RegisterHandler(groupElement.ConsolidatedUrl, cssHandler, minLastModified);
			});
		}

		private static void RegisterHandler(string consolidatedUrl, ResourceHandler handler, DateTime minLastModified)
		{
			handler.MinLastModified = minLastModified;
			_handlerRegistry.Add(UrlType.Dynamic.ConvertUrl(consolidatedUrl), handler);
		}

		private static IResourceCollector GetCollector(IResourceCollector element)
		{
			if (CachingEnabled)
				return new CachingResourceCollector(element);
			else
				return element;
		}

		private static bool CachingEnabled
		{
			get
			{
				return !Util.IsDebugMode;
			}
		}

		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		public void ProcessRequest(HttpContext context)
		{
			string appRelativePath = VirtualPathUtility.ToAppRelative(context.Request.Path);
			if (!_handlerRegistry.ContainsKey(appRelativePath))
				return;

			ResourceHandler handler = _handlerRegistry[appRelativePath];
			handler.HandleRequest(new HttpRequestContext(context));
		}

		/// <summary>
		/// 
		/// </summary>
		public bool IsReusable
		{
			get { return true; }
		}

		private class HttpRequestContext : IRequestContext
		{
			private HttpContext _httpContext;

			public HttpRequestContext(HttpContext httpContext)
			{
				_httpContext = httpContext;
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