using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

using AlmWitt.Web.ResourceManagement.ContentFiltering;

namespace AlmWitt.Web.ResourceManagement
{
	/// <summary>
	/// Represents a collection of <see cref="IResource"/>s.
	/// </summary>
	public class ResourceCollection : Collection<IResource>
	{
		public ResourceCollection() {}

    	/// <summary>
    	/// Constructs a new instance of a <see cref="ResourceCollection"/> that contains the given items.
    	/// </summary>
    	/// <param name="resources"></param>
		public ResourceCollection(IEnumerable<IResource> resources)
		{
			AddRange(resources);
		}

		/// <summary>
		/// Adds a collection of resources to this collection.
		/// </summary>
		/// <param name="resources"></param>
		public void AddRange(IEnumerable<IResource> resources)
		{
			foreach (IResource resource in resources)
			{
				Add(resource);
			}
		}

		/// <summary>
		/// Finds the first matching resource and returns it.  If one is not found
		/// then it returns null.
		/// </summary>
		/// <param name="match"></param>
		/// <returns></returns>
		public IResource FindOne(Predicate<IResource> match)
		{
			return this.SingleOrDefault(r => match(r));
		}

		/// <summary>
		/// Returns a <see cref="ResourceCollection"/> that matches the given criteria.
		/// </summary>
		/// <param name="match">A <see cref="Predicate{T}"/> predicate to match against.</param>
		/// <returns></returns>
		public ResourceCollection Where(Predicate<IResource> match)
		{
			return ((IEnumerable<IResource>) this)
				.Where(r => match(r))
				.ToResourceCollection();
		}

		/// <summary>
		/// Returns a sorted collection of resources.
		/// </summary>
		/// <param name="comparison">The comparison used for the sort.</param>
		/// <returns>The sorted collection.</returns>
		public ResourceCollection Sort(Comparison<IResource> comparison)
		{
			var list = new List<IResource>(this);
			list.Sort(comparison);
			
			return list.ToResourceCollection();
		}

		internal ResourceCollection Where(IResourceFilter filter)
		{
			return Where(filter.IsMatch);
		}

		internal ResourceCollection WhereNot(IResourceFilter excludeFilter)
		{
			return Where(ResourceFilters.Not(excludeFilter));
		}

		/// <summary>
		/// Returns the most recent last modified date of the contained resources.
		/// </summary>
		public DateTime LastModified
		{
			get
			{
				DateTime mostRecent = DateTime.MinValue;
				foreach (IResource resource in this)
				{
					if (resource.LastModified > mostRecent)
						mostRecent = resource.LastModified;
				}

				return mostRecent;
			}
		}

		/// <summary>
		/// Writes the contents of all of the contained resources to the given <see cref="TextWriter"/>.
		/// </summary>
		/// <param name="writer"></param>
		public void ConsolidateContentTo(TextWriter writer)
		{
			ConsolidateContentTo(writer, String.Empty);
		}

		/// <summary>
		/// Writes the contents of all of the contained resources to the given <see cref="TextWriter"/>.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="separator">A string that will be between each resource.</param>
		public void ConsolidateContentTo(TextWriter writer, string separator)
		{
			ConsolidateContentTo(writer, null, separator);
		}

		/// <summary>
		/// Writes the contents of all of the contained resources to the given <see cref="TextWriter"/>.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="createContentFilter"></param>
		public void ConsolidateContentTo(TextWriter writer, Func<IResource, IContentFilter> createContentFilter)
		{
			ConsolidateContentTo(writer, createContentFilter, null);
		}

		/// <summary>
		/// Writes the contents of all of the contained resources to the given <see cref="TextWriter"/>.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="createContentFilter"></param>
		/// <param name="separator">A string that will be between each resource.</param>
		public void ConsolidateContentTo(TextWriter writer, Func<IResource, IContentFilter> createContentFilter, string separator)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");
			if (createContentFilter == null)
				createContentFilter = r => NullContentFilter.Instance;
			if (separator == null)
				separator = String.Empty;

			bool first = true;
			foreach (IResource resource in this)
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
		public ConsolidatedResource Consolidate(Func<IResource,IContentFilter> createContentFilter, string separator)
		{
			var contentStream = new MemoryStream();
			var writer = new StreamWriter(contentStream);
			ConsolidateContentTo(writer, createContentFilter, separator);
			writer.Flush();

			return new ConsolidatedResource(this, contentStream);
		}

		/// <summary>
		/// Returns whether the given object is equal to this object.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			return Equals(obj as ResourceCollection);
		}

		/// <summary>
		/// Returns whether the given <see cref="ResourceCollection"/> contains the same (or equivilent) resources
		/// as this instance.
		/// </summary>
		/// <param name="resourceCollection"></param>
		/// <returns></returns>
		public bool Equals(ResourceCollection resourceCollection)
		{
			if (this == resourceCollection) return true;
			if (resourceCollection == null) return false;

			if (this.Count != resourceCollection.Count)
				return false;

			return this.All(resourceCollection.Contains);
		}

		/// <summary>
		/// Returns a hashcode generated from the contained resources.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return this.Count + this.Sum(resource => resource.GetHashCode());
		}
	}
}
