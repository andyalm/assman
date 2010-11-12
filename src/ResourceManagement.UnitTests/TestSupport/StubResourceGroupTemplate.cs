using System.Collections.Generic;
using System.Linq;

using AlmWitt.Web.ResourceManagement;
using AlmWitt.Web.ResourceManagement.Configuration;

namespace AlmWitt.Web.Test.ResourceManagement.TestObjects
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
			return Groups.Any(group => group.IsMatch(resource));
		}

		public bool MatchesConsolidatedUrl(string consolidatedUrl)
		{
			return Groups.Any(group => UrlType.ArePathsEqual(group.ConsolidatedUrl, consolidatedUrl));
		}

		public ResourceType ResourceType { get; set; }

		public bool Compress { get; set; }

		public IEnumerable<IResourceGroup> GetGroups(ResourceCollection allResources)
		{
			foreach (var group in _groups)
			{
				yield return new StaticResourceGroup(group.ConsolidatedUrl, allResources.Where(group));
			}
		}
	}
}