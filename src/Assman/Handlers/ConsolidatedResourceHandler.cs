using System;
using System.IO;

namespace Assman.Handlers
{
	internal class ConsolidatedResourceHandler : ResourceHandlerBase
	{
		private readonly string _path;
		private readonly ResourceCompiler _compiler;
		private readonly GroupTemplateContext _groupTemplateContext;
		private ResourceMode _resourceMode;
		private DateTime _minLastModified = DateTime.MinValue;
		private ICompiledResource _cachedConsolidatedResource;

		public ConsolidatedResourceHandler(string path, ResourceCompiler compiler, GroupTemplateContext groupTemplateContext, ResourceMode resourceMode)
		{
			_path = path;
			_compiler = compiler;
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

		private ICompiledResource GetConsolidatedResource()
		{
			if(_cachedConsolidatedResource != null)
			{
				return _cachedConsolidatedResource;
			}

			var consolidatedResource = _compiler.CompileGroup(_path, _groupTemplateContext, _resourceMode);
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
			private readonly ICompiledResource _compiledResource;
			private readonly ResourceMode _resourceMode;
			private readonly DateTime _minLastModified;
			private readonly ResourceType _resourceType;

			public ConsolidatedHandlerResource(ICompiledResource compiledResource, ResourceMode resourceMode,
											   DateTime minLastModified, ResourceType resourceType)
			{
				_compiledResource = compiledResource;
				_resourceMode = resourceMode;
				_minLastModified = minLastModified;
				_resourceType = resourceType;
			}

			public DateTime GetLastModified()
			{
				var lastModified = _compiledResource.LastModified;
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
					WriteSummaryHeader(outputStream);
				}

				_compiledResource.WriteTo(outputStream);
			}

            private void WriteSummaryHeader(Stream outputStream)
            {
                var writer = new StreamWriter(outputStream);
                try
                {
                    writer.Write("/*");
                    writer.WriteLine("This file consists of content from: ");
                    foreach (var resource in _compiledResource.Resources)
                    {
                        writer.WriteLine("\t" + resource.VirtualPath);
                    }
                }
                finally
                {
                    writer.WriteLine("*/");
                    writer.WriteLine();
                    writer.Flush();
                }
            }
		}
	}
}
