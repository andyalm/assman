using System;

namespace AlmWitt.Web.ResourceManagement
{
	public static class ResourceRegistryExtensions
	{
		public static void IncludeEmbeddedResource(this IResourceRegistry registry, Type type, string resourceName)
		{
			registry.IncludeEmbeddedResource(type.Assembly.GetName().Name, resourceName);
		}

		public static void IncludeEmbeddedResource(this IResourceRegistry registry, string assemblyName, string resourceName)
		{
			var url = registry.GetEmbeddedResourceUrl(assemblyName, resourceName);

			registry.IncludeUrl(url);
		}

		/// <summary>
		/// Registers an inline block that will appear inline on the page directly below the includes of this <see cref="IResourceRegistry"/>.
		/// </summary>
		/// <param name="block">The inline css or javascript that will appear on the page.</param>
		/// <param name="key">A unique key used to identify the inline block.  This is optional and can be set to <c>null</c>.</param>
		public static void RegisterInlineBlock(this IResourceRegistry registry, string block, object key)
		{
			registry.RegisterInlineBlock(w => w.Write(block), key);
		}
	}
}