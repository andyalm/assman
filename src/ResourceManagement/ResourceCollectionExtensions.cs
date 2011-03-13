using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using AlmWitt.Web.ResourceManagement.ContentFiltering;

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

		public static IEnumerable<IResource> Sort(this IEnumerable<IResource> resources, Comparison<IResource> comparison)
		{
			var list = new List<IResource>(resources);
			list.Sort(comparison);

			return list;
		}

		/// <summary>
		/// Adds a collection of resources to this collection.
		/// </summary>
		public static void AddRange(this IList<IResource> list, IEnumerable<IResource> resourcesToAdd)
		{
			foreach (IResource resource in resourcesToAdd)
			{
				list.Add(resource);
			}
		}

		public static IEnumerable<IResource> Where(this IEnumerable<IResource> resources, IResourceFilter filter)
		{
			return resources.Where(filter.IsMatch);
		}

		public static IEnumerable<IResource> Exclude(this IEnumerable<IResource> resources, IResourceFilter excludeFilter)
		{
			return resources.Where(ResourceFilters.Not(excludeFilter));
		}

		/// <summary>
		/// Returns the most recent last modified date of the contained resources.
		/// </summary>
		public static DateTime LastModified(this IEnumerable<IResource> resources)
		{
			DateTime mostRecent = DateTime.MinValue;
			foreach (IResource resource in resources)
			{
				if (resource.LastModified > mostRecent)
					mostRecent = resource.LastModified;
			}

			return mostRecent;
		}

		/// <summary>
		/// Writes the contents of all of the contained resources to the given <see cref="TextWriter"/>.
		/// </summary>
		public static void ConsolidateContentTo(this IEnumerable<IResource> resources, TextWriter writer)
		{
			resources.ConsolidateContentTo(writer, String.Empty);
		}

		/// <summary>
		/// Writes the contents of all of the contained resources to the given <see cref="TextWriter"/>.
		/// </summary>
		/// <param name="separator">A string that will be between each resource.</param>
		public static void ConsolidateContentTo(this IEnumerable<IResource> resources, TextWriter writer, string separator)
		{
			resources.ConsolidateContentTo(writer, null, separator);
		}

		/// <summary>
		/// Writes the contents of all of the contained resources to the given <see cref="TextWriter"/>.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="createContentFilter"></param>
		public static void ConsolidateContentTo(this IEnumerable<IResource> resources, TextWriter writer, Func<IResource, IContentFilter> createContentFilter)
		{
			resources.ConsolidateContentTo(writer, createContentFilter, null);
		}

		/// <summary>
		/// Writes the contents of all of the contained resources to the given <see cref="TextWriter"/>.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="createContentFilter"></param>
		/// <param name="separator">A string that will be between each resource.</param>
		public static void ConsolidateContentTo(this IEnumerable<IResource> resources, TextWriter writer, Func<IResource, IContentFilter> createContentFilter, string separator)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");
			if (createContentFilter == null)
				createContentFilter = r => NullContentFilter.Instance;
			if (separator == null)
				separator = String.Empty;

			bool first = true;
			foreach (IResource resource in resources)
			{
				if (first)
					first = false;
				else
					writer.Write(separator);
				string content = resource.GetContent();
				var contentFilter = createContentFilter(resource) ?? NullContentFilter.Instance;
				content = contentFilter.FilterContent(content);
				writer.Write(content);
			}
		}

		/// <summary>
		/// Consolidated all of the resources in the collection into a <see cref="ConsolidatedResource"/>.
		/// </summary>
		/// <param name="createContentFilter"></param>
		/// <param name="separator"></param>
		public static ConsolidatedResource Consolidate(this IEnumerable<IResource> resources, Func<IResource, IContentFilter> createContentFilter, string separator)
		{
			var contentStream = new MemoryStream();
			var writer = new StreamWriter(contentStream);
			resources.ConsolidateContentTo(writer, createContentFilter, separator);
			writer.Flush();

			return new ConsolidatedResource(resources.ToResourceCollection(), contentStream);
		}
	}
}