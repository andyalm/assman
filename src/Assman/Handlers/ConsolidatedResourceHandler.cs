using System;
using System.IO;

namespace Assman.Handlers
{
	internal class ConsolidatedResourceHandler : ResourceHandlerBase
	{
		private readonly string _path;
		private readonly ResourceConsolidator _consolidator;
		private readonly GroupTemplateContext _groupTemplateContext;
		private ResourceMode _resourceMode;
		private DateTime _minLastModified = DateTime.MinValue;
		private ConsolidatedResource _cachedConsolidatedResource;

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

		public ResourceMode ResourceMode
		{
			get { return _resourceMode; }
			set { _resourceMode = value; }
		}

		protected override IHandlerResource GetResource()
		{
			var consolidatedResource = GetConsolidatedResource();

			return new ConsolidatedHandlerResource(consolidatedResource, ResourceMode, MinLastModified, _groupTemplateContext.ResourceType);
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

		private class ConsolidatedHandlerResource : IHandlerResource
		{
			private readonly ConsolidatedResource _resource;
			private readonly ResourceMode _resourceMode;
			private readonly DateTime _minLastModified;
			private readonly ResourceType _resourceType;

			public ConsolidatedHandlerResource(ConsolidatedResource resource, ResourceMode resourceMode,
											   DateTime minLastModified, ResourceType resourceType)
			{
				_resource = resource;
				_resourceMode = resourceMode;
				_minLastModified = minLastModified;
				_resourceType = resourceType;
			}

			public DateTime GetLastModified()
			{
				var lastModified = _resource.LastModified;
				if (lastModified < _minLastModified)
					lastModified = _minLastModified;

				return lastModified;
			}

			public ResourceType ResourceType
			{
				get { return _resourceType; }
			}

			public void WriteContent(Stream outputStream)
			{
				if (_resourceMode == ResourceMode.Debug)
				{
					_resource.WriteSummaryHeader(outputStream);
				}

				_resource.WriteTo(outputStream);
			}
		}
	}
}
