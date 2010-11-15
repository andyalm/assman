using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace AlmWitt.Web.ResourceManagement.Configuration
{
	/// <summary>
	/// Represents the base class for configuring a specific type of resource.
	/// </summary>
	public abstract class ResourceGroupElement : ConfigurationElement, IResourceGroupTemplate
	{
		private ConsolidatedUrlTemplate _consolidatedUrlTemplate;
		
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
			set
			{
				this[PropertyNames.ConsolidatedUrl] = value;
				_consolidatedUrlTemplate = null;
			}
		}

		private bool? _compress;

		/// <summary>
		/// Gets or sets whether the scripts will be compressed when they are consolidated.
		/// </summary>
		public bool Compress
		{
			get
			{
				return _compress ?? CompressDefaultValue;
			}
			set { _compress = value; }
		}

		private ConsolidatedUrlTemplate ConsolidatedUrlTemplate
		{
			get
			{
				if (_consolidatedUrlTemplate == null)
					_consolidatedUrlTemplate = ConsolidatedUrlTemplate.GetInstance(ConsolidatedUrl);

				return _consolidatedUrlTemplate;
			}
		}

		internal bool CompressDefaultValue { get; set; }

		public abstract ResourceType ResourceType { get; }

		protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
		{
			bool compress;
			if (name == PropertyNames.Compress && Boolean.TryParse(value, out compress))
			{
				_compress = compress;
				return true;
			}
			else
			{
				return base.OnDeserializeUnrecognizedAttribute(name, value);
			}
		}

		public bool MatchesConsolidatedUrl(string consolidatedUrl)
		{
			return ConsolidatedUrlTemplate.Matches(consolidatedUrl);
		}

		public IEnumerable<IResourceGroup> GetGroups(ResourceCollection allResources)
		{
			return  from resource in allResources
					let match = GetMatch(resource.VirtualPath)
					where match.IsMatch
					let resourceWithUrl = new { Resource = resource, ConsolidatedUrl = GetConsolidatedUrl(match) }
					group resourceWithUrl by resourceWithUrl.ConsolidatedUrl into @group
					select CreateGroup(@group.Key,
						@group.Select(g => g.Resource).Sort(IncludePatternOrder()));
		}

		public string GetResourceUrl(string resourceUrl, UrlType urlType)
		{
			if (Consolidate && IsMatch(resourceUrl))
				return urlType.ConvertUrl(ConsolidatedUrl);
			else
				return resourceUrl;
		}

		private string GetConsolidatedUrl(IResourceMatch match)
		{
			return ConsolidatedUrlTemplate.Format(match);
		}

		private IResourceGroup CreateGroup(string consolidatedUrl, IEnumerable<IResource> resourcesInGroup)
		{
			return new StaticResourceGroup(consolidatedUrl, resourcesInGroup)
			{
				Compress = this.Compress
			};
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

		internal IResourceMatch GetMatch(string resourceUrl)
		{
			if(Include.Count == 0)
			{
				return Exclude.GetMatch(resourceUrl).Inverse();
			}

			var includeMatch = Include.GetMatch(resourceUrl);
			var excludeMatch = Exclude.GetMatch(resourceUrl);
			if (includeMatch.IsMatch && !excludeMatch.IsMatch)
				return includeMatch;

			return ResourceMatches.False(resourceUrl);
		}

		private Comparison<IResource> IncludePatternOrder()
		{
			return (IResource x, IResource y) =>
			{
				int xIndex = Include.GetMatchIndex(x.VirtualPath);
				int yIndex = Include.GetMatchIndex(y.VirtualPath);

				return xIndex - yIndex;
			};
		}

		private static class PropertyNames
		{
			public const string Consolidate = "consolidate";
			public const string Exclude = "exclude";
			public const string Include = "include";
			public const string Compress = "compress";
			public const string ConsolidatedUrl = "consolidatedUrl";
		}
	}
}
