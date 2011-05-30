using System;
using System.IO;

namespace Assman.Handlers
{
	internal class ConsolidatedResourceHandler : ResourceHandlerBase
	{
		private readonly string _path;
		private readonly ResourceCompiler _compiler;
		private readonly GroupTemplateContext _groupTemplateContext;
		private DateTime _minLastModified = DateTime.MinValue;

	    public ConsolidatedResourceHandler(string path, ResourceCompiler compiler, GroupTemplateContext groupTemplateContext)
		{
			_path = path;
			_compiler = compiler;
			_groupTemplateContext = groupTemplateContext;
		}

		public DateTime MinLastModified
		{
			get { return _minLastModified; }
			set { _minLastModified = value.ToUniversalTime(); }
		}

		protected override IHandlerResource GetResource()
		{
			var consolidatedResource = GetConsolidatedResource();

			return new ConsolidatedHandlerResource(consolidatedResource, Mode, MinLastModified, _groupTemplateContext.ResourceType);
		}

		private ICompiledResource GetConsolidatedResource()
		{
			return _compiler.CompileGroup(_path, _groupTemplateContext, Mode);
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
