using System;
using System.IO;

using Assman.Configuration;

namespace Assman.Registration
{
	public static class ResourceRegistryExtensions
	{
		public static void RequireEmbeddedResource(this IResourceRegistry registry, Type type, string resourceName)
		{
			registry.RequireEmbeddedResource(type.Assembly.GetName().Name, resourceName);
		}

		public static void RequireEmbeddedResource(this IResourceRegistry registry, string assemblyName, string resourceName)
		{
			var shortAssemblyName = assemblyName.ToShortAssemblyName();
			if (!AssmanConfiguration.Current.Assemblies.Contains(shortAssemblyName))
			{
				throw new InvalidOperationException(@"Cannot include embedded resource because the assembly has not been configured in the Assman.config.  If you would like to embed a resource from the assembly '"
													+ assemblyName + "' then please add it to the <assemblies> list.");
			}
			var embeddedResourceVirtualPath = EmbeddedResource.GetVirtualPath(shortAssemblyName, resourceName);
			registry.Require(embeddedResourceVirtualPath);
		}

		/// <summary>
		/// Registers an inline block that will appear inline on the page directly below the includes of this <see cref="IResourceRegistry"/>.
		/// </summary>
		/// <param name="registry"></param>
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
	}
}