using System.Collections.Generic;

namespace AlmWitt.Web.ResourceManagement
{
	public interface IResourceGroup : IResourceFilter
	{
		string ConsolidatedUrl { get; }
		
		bool Compress { get; }

		/// <summary>
		/// Gets the resources that belong to the group.
		/// </summary>
		/// <param name="finder">A <see cref="IResourceFinder"/> that finds all resources that are available.</param>
		IEnumerable<IResource> GetResources();
	}
}