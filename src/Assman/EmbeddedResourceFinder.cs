using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Assman
{
	internal class EmbeddedResourceFinder : IResourceFinder
	{
		private readonly Assembly _assembly;

		internal EmbeddedResourceFinder(Assembly assembly)
		{
			_assembly = assembly;
		}

		public IEnumerable<IResource> FindResources(ResourceType resourceType)
		{
			return from resourceName in _assembly.GetManifestResourceNames()
			        where resourceType.FileExtensions.Any(resourceName.EndsWith)
			        select CreateEmbeddedResource(resourceName);
		}

		public IResource FindResource(string virtualPath)
		{
			//TODO: Unit test this method
			if(!EmbeddedResource.IsVirtualPath(virtualPath))
			{
				return null;
			}

			var uri = new Uri(virtualPath);
			if(!uri.Host.EqualsVirtualPath(_assembly.FullName.ToShortAssemblyName()))
			{
				return null;
			}

			var resourceName = uri.PathAndQuery.Substring(1);
			if (_assembly.GetManifestResourceInfo(resourceName) != null)
				return new EmbeddedResource(_assembly, resourceName);

			return null;
		}

		private IResource CreateEmbeddedResource(string resourceName)
		{
			return new EmbeddedResource(_assembly, resourceName);
		}
	}
}
