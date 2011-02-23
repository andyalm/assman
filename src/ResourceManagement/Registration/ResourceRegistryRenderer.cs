using System;
using System.IO;
using System.Linq;

namespace AlmWitt.Web.ResourceManagement.Registration
{
	public class ResourceRegistryRenderer
	{
		private readonly RegisteredResources _resources;
		private readonly IResourceWriter _resourceWriter;
		private readonly Func<string, string> _urlResolver;

		public ResourceRegistryRenderer(RegisteredResources resources, IResourceWriter resourceWriter, Func<string,string> urlResolver)
		{
			_resources = resources;
			_resourceWriter = resourceWriter;
			_urlResolver = urlResolver;
		}

		public void Render(TextWriter writer)
		{
			foreach (var includeUrl in _resources.Includes)
			{
				var resolvedUrl = _urlResolver(includeUrl);
				_resourceWriter.WriteIncludeTag(writer, resolvedUrl);
			}

			var scriptBlocks = _resources.InlineBlocks;
			if (scriptBlocks.Any())
			{
				_resourceWriter.WriteBeginBlock(writer);
				foreach (var scriptBlock in scriptBlocks)
				{
					scriptBlock(writer);
				}
				_resourceWriter.WriteEndBlock(writer);
			}
		}
	}

	public static class ResourceRegistryRendererExtensions
	{
		private static readonly IResourceWriter _scriptWriter = new ScriptResourceWriter();
		private static readonly IResourceWriter _styleWriter = new StyleResourceWriter();
		
		public static ResourceRegistryRenderer ScriptRenderer(this IResourceRegistryAccessor registryAccessor, string registryName, Func<string,string> urlResolver)
		{
			var registeredResources = registryAccessor.GetRegisteredScripts(registryName);
			
			return new ResourceRegistryRenderer(registeredResources, _scriptWriter, urlResolver);
		}

		public static ResourceRegistryRenderer StyleRenderer(this IResourceRegistryAccessor registryAccessor, string registryName, Func<string,string> urlResolver)
		{
			var registeredResources = registryAccessor.GetRegisteredStyles(registryName);

			return new ResourceRegistryRenderer(registeredResources, _styleWriter, urlResolver);
		}
		
		private class ScriptResourceWriter : IResourceWriter
		{
			public void WriteIncludeTag(TextWriter writer, string url)
			{
				writer.Write("<script type=\"text/javascript\" src=\"{0}\"></script>", url);
			}

			public void WriteBeginBlock(TextWriter writer)
			{
				writer.Write("<script type=\"text/javascript\">");
			}

			public void WriteEndBlock(TextWriter writer)
			{
				writer.Write("</script>");
			}
		}

		private class StyleResourceWriter : IResourceWriter
		{
			public void WriteIncludeTag(TextWriter writer, string url)
			{
				writer.Write("<link rel=\"Stylesheet\" type=\"text/css\" href=\"{0}\"></link>", url);
			}

			public void WriteBeginBlock(TextWriter writer)
			{
				writer.Write("<style type=\"text/css\">");
			}

			public void WriteEndBlock(TextWriter writer)
			{
				writer.Write("</style>");
			}
		}
	}
}