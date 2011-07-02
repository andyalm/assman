using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Assman.Configuration
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
		[ConfigurationProperty(PropertyNames.Consolidate, IsRequired = false, DefaultValue = ResourceModeCondition.Always)]
		public ResourceModeCondition Consolidate
		{
			get { return (ResourceModeCondition)this[PropertyNames.Consolidate]; }
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

		private ResourceModeCondition? _minify;

		/// <summary>
		/// Gets or sets whether the scripts/styles will be minified when they are consolidated in Release mode.
		/// </summary>
		public ResourceModeCondition Minify
		{
			get
			{
				return _minify ?? MinifyDefaultValue;
			}
			set { _minify = value; }
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

		internal ResourceModeCondition MinifyDefaultValue { get; set; }

		public abstract ResourceType ResourceType { get; }

		protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
		{
			ResourceModeCondition minify;
			if (name == PropertyNames.Minify && value.TryConvertTo(out minify))
			{
				_minify = minify;
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

		public IEnumerable<IResourceGroup> GetGroups(IEnumerable<IResource> allResources, ResourceMode mode)
		{
			//prevent multiple enumerations of potentially expensive IEnumerable
			allResources = allResources.ToList();

			var externallyCompiledResources = allResources.ExternallyCompiled();
			
			return  from resource in allResources
					let match = GetMatch(resource.VirtualPath)
					let resourceOfCorrectMode = resource.WithMode(mode, externallyCompiledResources)
					let relatedMatch = GetMatch(resourceOfCorrectMode.VirtualPath)
					let isMatch = match.IsMatch()
					let isRelatedMatch = relatedMatch.IsMatch()
					where isMatch || isRelatedMatch
					let resourceWithUrl = new ResourceWithMatchingPath
					{
						Resource = resourceOfCorrectMode,
						ConsolidatedUrl = GetConsolidatedUrl(match),
						MatchingPath = isMatch ? resource.VirtualPath : resourceOfCorrectMode.VirtualPath
					}
					group resourceWithUrl by resourceWithUrl.ConsolidatedUrl into @group
					select CreateGroup(@group.Key,
						@group.Distinct().Sort(IncludePatternOrder()).Select(r => r.Resource), mode);
		}

		private class ResourceWithMatchingPath
		{
			public IResource Resource { get; set; }
			public string ConsolidatedUrl { get; set; }
			public string MatchingPath { get; set; }

			public override bool Equals(object obj)
			{
				return Resource.VirtualPath.Equals(((ResourceWithMatchingPath)obj).Resource.VirtualPath);
			}

			public override int GetHashCode()
			{
				return Resource.VirtualPath.GetHashCode();
			}
		}

		public bool TryGetConsolidatedUrl(string virtualPath, ResourceMode resourceMode, out string consolidatedUrl)
		{
			consolidatedUrl = null;
			if (Consolidate.IsFalse(resourceMode))
				return false;

			var match = GetMatch(virtualPath);
			if(match.IsMatch())
			{
				consolidatedUrl = GetConsolidatedUrl(match);
				return true;
			}

			return false;
		}

		private string GetConsolidatedUrl(IResourceMatch match)
		{
			return ConsolidatedUrlTemplate.Format(match);
		}

		private IResourceGroup CreateGroup(string consolidatedUrl, IEnumerable<IResource> resourcesInGroup, ResourceMode resourceMode)
		{
			return new ResourceGroup(consolidatedUrl, resourcesInGroup)
			{
				Minify = this.Minify.IsTrue(resourceMode)
			};
		}

		public bool IsMatch(IResource resource)
		{
			return IsMatch(resource.VirtualPath);
		}

		public bool IsMatch(string resourceUrl)
		{
			return (Include.Count == 0 || Include.IsMatch(resourceUrl))
				&& !Exclude.IsMatch(resourceUrl);
		}

		private IResourceMatch GetMatch(string resourceUrl)
		{
			if(Include.Count == 0)
			{
				return Exclude.GetMatch(resourceUrl).Inverse();
			}

			var includeMatch = Include.GetMatch(resourceUrl);
			var excludeMatch = Exclude.GetMatch(resourceUrl);
			if (includeMatch.IsMatch() && !excludeMatch.IsMatch())
				return includeMatch;

			return ResourceMatches.False(resourceUrl);
		}

		private Comparison<ResourceWithMatchingPath> IncludePatternOrder()
		{
			return (ResourceWithMatchingPath x, ResourceWithMatchingPath y) =>
			{
				int xIndex = Include.GetMatchIndex(x.MatchingPath);
				int yIndex = Include.GetMatchIndex(y.MatchingPath);

				return xIndex - yIndex;
			};
		}
	}
}
