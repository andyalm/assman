using System.Collections.Generic;

namespace AlmWitt.Web.ResourceManagement
{
	public interface IResourceGroup
	{
		/// <summary>
		/// The url of the file that the contents of all resources in the group are consolidated to.
		/// </summary>
		string ConsolidatedUrl { get; }
		
		/// <summary>
		/// Indicates whether compression is enabled for the content of the resources in this resource group.
		/// </summary>
		bool Compress { get; }

		/// <summary>
		/// Indicates whether the given resource exists in the resource group.
		/// </summary>
		bool Contains(IResource resource);
		
		/// <summary>
		/// Gets the resources that belong to the group.
		/// </summary>
		/// <param name="finder">A <see cref="IResourceFinder"/> that finds all resources that are available.</param>
		IEnumerable<IResource> GetResources();
	}
}