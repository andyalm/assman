using System;
using System.Linq;
using System.Reflection;

namespace AlmWitt.Web.ResourceManagement
{
	internal class EmbeddedResourceFinder : IResourceFinder
	{
		private readonly Assembly _assembly;

		internal EmbeddedResourceFinder(Assembly assembly)
		{
			_assembly = assembly;
		}

		public ResourceCollection FindResources(ResourceType resourceType)
		{
			return (from resourceName in _assembly.GetManifestResourceNames()
			        where resourceType.FileExtensions.Any(resourceName.EndsWith)
			        select CreateEmbeddedResource(resourceName)).ToResourceCollection();
		}

		private IResource CreateEmbeddedResource(string resourceName)
		{
			return new EmbeddedResource(_assembly, resourceName);
		}
	}
}
