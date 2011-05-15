using System;
using System.Web;

using Assman.Configuration;

namespace Assman.Handlers
{
	internal class ConsolidatedResourceHandler : HttpHandlerBase
	{
		private readonly string _path;
		private readonly ResourceConsolidator _consolidator;
		private readonly GroupTemplateContext _groupTemplateContext;
	    private readonly ResourceMode _resourceMode;
	    private DateTime _minLastModified = DateTime.MinValue;
		private ConsolidatedResource _cachedConsolidatedResource;
		internal Func<IConfigLoader> GetConfigurationLoader = () => new DefaultConfigLoader();

		public ConsolidatedResourceHandler(string path, ResourceConsolidator consolidator, GroupTemplateContext groupTemplateContext, ResourceMode resourceMode)
		{
			_path = path;
			_consolidator = consolidator;
			_groupTemplateContext = groupTemplateContext;
		    _resourceMode = resourceMode;
		}

		public DateTime MinLastModified
		{
			get { return _minLastModified; }
			set { _minLastModified = value.ToUniversalTime(); }
		}

		public override void ProcessRequest(HttpContextBase context)
		{
			HandleRequest(new HttpRequestContext(context));   
		}

		public void HandleRequest(IRequestContext context)
		{
			var resource = GetConsolidatedResource();
			DateTime lastModified = resource.LastModified.ToUniversalTime();
			if (lastModified < _minLastModified)
				lastModified = _minLastModified;
			DateTime ifModifiedSince = (context.IfModifiedSince ?? DateTime.MinValue).ToUniversalTime();
			lastModified = RoundToSeconds(lastModified);
			ifModifiedSince = RoundToSeconds(ifModifiedSince);
			if (lastModified <= ifModifiedSince)
			{
				context.StatusCode = 304;
				context.StatusDescription = "Not Modified";
				return;
			}
			else
			{
				context.SetLastModified(lastModified);
				context.ContentType = _groupTemplateContext.ResourceType.ContentType;
				
				if(IsDebugMode)
				{
					resource.WriteSummaryHeader(context.OutputStream);
				}
				
				resource.WriteTo(context.OutputStream);
			}
		}

		private static DateTime RoundToSeconds(DateTime dateTime)
		{
			return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
		}

		private ConsolidatedResource GetConsolidatedResource()
		{
			if(_cachedConsolidatedResource != null)
			{
				return _cachedConsolidatedResource;
			}

			var consolidatedResource = _consolidator.ConsolidateGroup(_path, _groupTemplateContext, _resourceMode);
			if(CachingEnabled)
			{
				_cachedConsolidatedResource = consolidatedResource;
			}

			return consolidatedResource;
		}

		private bool CachingEnabled
		{
			get { return !IsDebugMode; }
		}

		private bool IsDebugMode
		{
			get { return _resourceMode == ResourceMode.Debug; }
		}
	}
}
