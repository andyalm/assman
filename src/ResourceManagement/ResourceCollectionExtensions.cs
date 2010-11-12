using System;
using System.Collections.Generic;

namespace AlmWitt.Web.ResourceManagement
{
	public static class ResourceCollectionExtensions
	{
		public static ResourceCollection ToResourceCollection(this IEnumerable<IResource> resources)
		{
			var resourceCollection = new ResourceCollection();
			resourceCollection.AddRange(resources);

			return resourceCollection;
		}

		public static ResourceCollection Sort(this IEnumerable<IResource> resources, Comparison<IResource> comparison)
		{
			return resources.ToResourceCollection().Sort(comparison);
		}
	}
}