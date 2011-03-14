using System;
using System.Web.Configuration;

using AlmWitt.Web.ResourceManagement.Configuration;

namespace AlmWitt.Web.ResourceManagement
{
	internal class ResourceHandler : IResourceHandler
	{
		private readonly string _path;
		private readonly ResourceManagementContext _configContext;
		private readonly GroupTemplateContext _groupTemplateContext;
		private DateTime _minLastModified = DateTime.MinValue;
		private ConsolidatedResource _cachedConsolidatedResource;
		internal Func<IConfigLoader> GetConfigurationLoader = () => new DefaultConfigLoader();

		public ResourceHandler(string path, ResourceManagementContext configContext, GroupTemplateContext groupTemplateContext)
		{
			_path = path;
			_configContext = configContext;
			_groupTemplateContext = groupTemplateContext;
		}

		public DateTime MinLastModified
		{
			get { return _minLastModified; }
			set { _minLastModified = value.ToUniversalTime(); }
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

			var consolidatedResource = _configContext.ConsolidateGroup(_path, _groupTemplateContext, IsDebugMode ? ResourceMode.Debug : ResourceMode.Release);
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
			get
			{
				var compilationSection = GetConfigurationLoader().GetSection<CompilationSection>("system.web/compilation");
				return compilationSection.Debug;
			}
		}
	}
}
