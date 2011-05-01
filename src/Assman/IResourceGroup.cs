using System.Collections.Generic;

namespace Assman
{
	public interface IResourceGroup
	{
		/// <summary>
		/// The url of the file that the contents of all resources in the group are consolidated to.
		/// </summary>
		string ConsolidatedUrl { get; }
		
		/// <summary>
		/// Indicates whether to minify the resources contents when in Release mode.
		/// </summary>
		bool Minify { get; }

		/// <summary>
		/// Gets the <see cref="ResourceType"/> of the resources in the group.
		/// </summary>
		ResourceType ResourceType { get; }

		/// <summary>
		/// Indicates whether the given resource exists in the resource group.
		/// </summary>
		bool Contains(IResource resource);
		
		/// <summary>
		/// Gets the resources that belong to the group.
		/// </summary>
		IEnumerable<IResource> GetResources();
	}

	public static class ResourceGroupExtensions
	{
		public static bool ShouldMinify(this IResourceGroup group, ResourceMode mode)
		{
			return group.Minify && mode == ResourceMode.Release;
		}
	}
}