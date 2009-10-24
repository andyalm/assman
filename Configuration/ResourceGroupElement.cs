using System;
using System.Configuration;
using AlmWitt.Web.ResourceManagement.ContentFiltering;

namespace AlmWitt.Web.ResourceManagement.Configuration
{
	/// <summary>
	/// Represents the base class for configuring a specific type of resource.
	/// </summary>
	public abstract class ResourceGroupElement : ConfigurationElement, IResourceCollector, IResourceFilter
	{
		/// <summary>
		/// Gets or sets the resources that are to be included from consolidation.
		/// </summary>
		[ConfigurationProperty(PropertyNames.Include, IsRequired = false)]
		public ResourceMatchElementCollection Include
		{
			get { return this[PropertyNames.Include] as ResourceMatchElementCollection; }
			set { this[PropertyNames.Include] = value; }
		}

		/// <summary>
		/// Gets or sets the resources that are to be excluded from consolidation.
		/// </summary>
		[ConfigurationProperty(PropertyNames.Exclude, IsRequired = false)]
		public ResourceMatchElementCollection Exclude
		{
			get { return this[PropertyNames.Exclude] as ResourceMatchElementCollection; }
			set { this[PropertyNames.Exclude] = value; }
		}

		/// <summary>
		/// Gets or sets whether consolidation is enabled for this group.
		/// </summary>
		[ConfigurationProperty(PropertyNames.Consolidate, IsRequired = false, DefaultValue = true)]
		public bool Consolidate
		{
			get { return (bool)this[PropertyNames.Consolidate]; }
			set { this[PropertyNames.Consolidate] = value; }
		}

		/// <summary>
		/// Gets the url that is used for consolidated resources of this type.
		/// </summary>
		[ConfigurationProperty(PropertyNames.ConsolidatedUrl, IsRequired = true)]
		public string ConsolidatedUrl
		{
			get { return (string)this[PropertyNames.ConsolidatedUrl]; }
			set { this[PropertyNames.ConsolidatedUrl] = value; }
		}

		/// <summary>
		/// Gets the <see cref="IContentFilter"/> used to filter the content when it is consolidated.
		/// </summary>
		protected virtual IContentFilter ContentFilter
		{
			get { return null; }
		}

		/// <summary>
		/// Gets the separator string that is put between resources when consolidated.
		/// </summary>
		protected virtual string Separator
		{
			get { return Environment.NewLine; }
		}

		ConsolidatedResource IResourceCollector.GetResource(IResourceFinder finder, string extension, IResourceFilter exclude)
		{
			return GetResource(finder, extension, exclude);
		}

		internal ConsolidatedResource GetResource(IResourceFinder finder, string extension)
		{
			return GetResource(finder, extension, null);
		}

		internal ConsolidatedResource GetResource(IResourceFinder finder, string extension, IResourceFilter exclude)
		{
			if (exclude == null)
				exclude = ResourceFilters.False;
            ResourceCollection resources = finder.FindResources(extension).Where(IsMatch);
			resources = resources.Where(ResourceFilters.Not(exclude));
			resources = resources.Sort(delegate(IResource x, IResource y)
			{
				return Include.GetMatchIndex(x.VirtualPath) - Include.GetMatchIndex(y.VirtualPath);
			});
			return ConsolidatedResource.FromResources(resources, ContentFilter, Separator);
		}

		internal string GetResourceUrl(string resourceUrl, UrlType urlType)
		{
			if (Consolidate && IsMatch(resourceUrl))
				return urlType.ConvertUrl(ConsolidatedUrl);
			else
				return resourceUrl;
		}

		bool IResourceFilter.IsMatch(IResource resource)
		{
			return IsMatch(resource);
		}

		internal bool IsMatch(IResource resource)
		{
			return IsMatch(resource.VirtualPath);
		}

		internal bool IsMatch(string resourceUrl)
		{
			return (Include.Count == 0 || Include.IsMatch(resourceUrl))
				&& !Exclude.IsMatch(resourceUrl);
		}

		private static class PropertyNames
		{
			public const string Consolidate = "consolidate";
			public const string Exclude = "exclude";
			public const string Include = "include";
			public const string ConsolidatedUrl = "consolidatedUrl";
		}
	}
}