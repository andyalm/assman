using System;
using System.Collections.Generic;
using System.Linq;

using AlmWitt.Web.ResourceManagement.Configuration;

namespace AlmWitt.Web.ResourceManagement.TestObjects
{
	public class StubResourceGroupTemplate : IResourceGroupTemplate
	{
		private readonly List<IResourceGroup> _groups = new List<IResourceGroup>();

		public List<IResourceGroup> Groups
		{
			get { return _groups; }
		}

		public StubResourceGroupTemplate(IResourceGroup group)
		{
			if(group != null)
				_groups.Add(group);
		}

		public bool IsMatch(IResource resource)
		{
			return Groups.Any(group => group.Contains(resource));
		}

		public bool MatchesConsolidatedUrl(string consolidatedUrl)
		{
			return Groups.Any(group => UrlType.ArePathsEqual(group.ConsolidatedUrl, consolidatedUrl));
		}

		public ResourceType ResourceType { get; set; }

		public bool Compress { get; set; }

		public IEnumerable<IResourceGroup> GetGroups(ResourceCollection allResources)
		{
			return from @group in _groups
				   select CreateGroup(@group.ConsolidatedUrl, allResources.Where(@group.Contains));
		}

		public bool TryGetConsolidatedUrl(string virtualPath, out string consolidatedUrl)
		{
			throw new NotImplementedException();
		}

		private IResourceGroup CreateGroup(string consolidatedUrl, IEnumerable<IResource> resources)
		{
			return new ResourceGroup(consolidatedUrl, resources)
			{
				Compress = this.Compress
			};
		}
	}
}