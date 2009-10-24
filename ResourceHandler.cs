using System;

namespace AlmWitt.Web.ResourceManagement
{
	internal abstract class ResourceHandler
	{
		private IResourceFinder _finder;
		private IResourceCollector _collector;
		private IResourceFilter _excludeFilter;
		private DateTime _minLastModified = DateTime.MinValue;


		protected ResourceHandler(IResourceFinder finder, IResourceCollector collector, IResourceFilter excludeFilter)
		{
            _finder = finder;
			_collector = collector;
			_excludeFilter = excludeFilter;
		}

		public DateTime MinLastModified
		{
			get { return _minLastModified; }
			set { _minLastModified = value.ToUniversalTime(); }
		}

		protected abstract string Extension { get; }
		protected abstract string ContentType { get; }

		public void HandleRequest(IRequestContext context)
		{
			ConsolidatedResource resource = _collector.GetResource(_finder, Extension, _excludeFilter);
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
				context.ContentType = ContentType;
				resource.ContentStream.WriteTo(context.OutputStream);
			}
		}

		private static DateTime RoundToSeconds(DateTime dateTime)
		{
			return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
		}
	}

	internal class ClientScriptResourceHandler : ResourceHandler
	{
		public ClientScriptResourceHandler(IResourceFinder finder, IResourceCollector collector, IResourceFilter excludeFilter) : base(finder, collector, excludeFilter) { }

		protected override string Extension
		{
			get { return ".js"; }
		}

		protected override string ContentType
		{
			get { return "text/javascript"; }
		}
	}

	internal class CssResourceHandler : ResourceHandler
	{
		public CssResourceHandler(IResourceFinder finder, IResourceCollector collector, IResourceFilter excludeFilter) : base(finder, collector, excludeFilter) { }

		protected override string Extension
		{
			get { return ".css"; }
		}

		protected override string ContentType
		{
			get { return "text/css"; }
		}
	}
}