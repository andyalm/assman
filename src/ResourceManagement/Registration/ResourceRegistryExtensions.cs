using System;
using System.IO;

using AlmWitt.Web.ResourceManagement.Configuration;

namespace AlmWitt.Web.ResourceManagement.Registration
{
	public static class ResourceRegistryExtensions
	{
		public static void IncludeEmbeddedResource(this IResourceRegistry registry, Type type, string resourceName)
		{
			registry.IncludeEmbeddedResource(type.Assembly.GetName().Name, resourceName);
		}

		public static void IncludeEmbeddedResource(this IResourceRegistry registry, string assemblyName, string resourceName)
		{
			var url = GetEmbeddedResourceUrl(registry, assemblyName, resourceName);

			registry.IncludePath(url);
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

		public static void RegisterInlineBlock(this IResourceRegistry registry, Action<TextWriter> block)
		{
			registry.RegisterInlineBlock(block, null);
		}

		public static void RegisterInlineBlock(this IResourceRegistry registry, string block)
		{
			registry.RegisterInlineBlock(block, null);
		}

		private static string GetEmbeddedResourceUrl(IResourceRegistry registry, string assemblyName, string resourceName)
		{
			string shortAssemblyName = assemblyName.ToShortAssemblyName();
			string virtualPath = EmbeddedResource.GetVirtualPath(shortAssemblyName, resourceName);
			string consolidatedUrl;
			if (!registry.TryResolvePath(virtualPath, out consolidatedUrl))
			{
				throw new InvalidOperationException(
					@"Cannot include embedded resource because it has not been configured in the ResourceManagement.config to be consolidated anywhere.
					Please add an include rule that matches the path 'assembly://" +
					assemblyName + "/" + resourceName + "'.");
			}
			if(!ResourceManagementConfiguration.Current.Assemblies.Contains(shortAssemblyName))
			{
				throw new InvalidOperationException(@"Cannot include embedded resource because the assembly has not been configured in the ResourceManagement.config.  If you would like to embed a resource from the assembly '"
				                                    + assemblyName + "' then please add it to the <assemblies> list.");
			}

			return consolidatedUrl;
		}
	}
}