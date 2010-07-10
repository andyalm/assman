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
    	/// <summary>
    	/// Constructs a new instance of an empty <see cref="ResourceCollection"/>.
    	/// </summary>
		public ResourceCollection()
    	{
    	}

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
        /// Finds the first resource in the collection that matches the given predicate.
        /// </summary>
        /// <param name="match">A <see cref="Predicate{T}"/> predicate to match against.</param>
        /// <returns></returns>
        public IResource Find(Predicate<IResource> match)
        {
            ResourceCollection matches = Where(match);
            if (matches.Count > 0)
                return matches[0];
            else
                return null;
        }

        /// <summary>
        /// Returns a <see cref="ResourceCollection"/> that matches the given criteria.
        /// </summary>
        /// <param name="match">A <see cref="Predicate{T}"/> predicate to match against.</param>
        /// <returns></returns>
        public ResourceCollection Where(Predicate<IResource> match)
        {
            ResourceCollection filteredCollection = new ResourceCollection();
            foreach (IResource resource in this)
            {
                if (match(resource))
                    filteredCollection.Add(resource);
            }

            return filteredCollection;
        }

        internal ResourceCollection Where(IResourceFilter filter)
        {
            return Where(filter.IsMatch);
        }

        /// <summary>
        /// Writes the contents of all of the contained resources to the given <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="writer"></param>
        public void Consolidate(TextWriter writer)
        {
            Consolidate(writer, String.Empty);
        }

        /// <summary>
        /// Writes the contents of all of the contained resources to the given <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="separator">A string that will be between each resource.</param>
        public void Consolidate(TextWriter writer, string separator)
        {
            Consolidate(writer, null, separator);
        }

        /// <summary>
        /// Writes the contents of all of the contained resources to the given <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="filter"></param>
        public void Consolidate(TextWriter writer, IContentFilter filter)
        {
            Consolidate(writer, filter, null);
        }

        /// <summary>
        /// Writes the contents of all of the contained resources to the given <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="filter"></param>
        /// <param name="separator">A string that will be between each resource.</param>
        public void Consolidate(TextWriter writer, IContentFilter filter, string separator)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");
            if (filter == null)
                filter = NullContentFilter.Instance;
            if (separator == null)
                separator = String.Empty;

            StringWriter tempWriter = new StringWriter();

            bool first = true;
            foreach (IResource resource in this)
            {
                if (first)
                    first = false;
                else
                    tempWriter.Write(separator);
                tempWriter.Write(resource.GetContent());
            }

            string consolidatedContent = tempWriter.ToString();
            consolidatedContent = filter.FilterContent(consolidatedContent);
            writer.Write(consolidatedContent);
            writer.Flush();
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

            foreach (IResource resource in this)
            {
                if (!resourceCollection.Contains(resource))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Returns a hashcode generated from the contained resources.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            int hash = this.Count;
            foreach (IResource resource in this)
            {
                hash += resource.GetHashCode();
            }

            return hash;
        }

    	/// <summary>
    	/// Returns a sorted copy of the <see cref="ResourceCollection"/>.
    	/// </summary>
    	/// <param name="comparison">The comparison to use to sort.</param>
    	/// <returns></returns>
		public ResourceCollection Sort(Comparison<IResource> comparison)
    	{
    		List<IResource> list = new List<IResource>(this);
			list.Sort(comparison);

			return new ResourceCollection(list);
    	}
    }

    public static class ResourceCollectionExtensions
    {
        public static ResourceCollection ToResourceCollection<TResource>(this IEnumerable<TResource> resources) where TResource : IResource
        {
            return new ResourceCollection(resources.Cast<IResource>());
        }
    }
}