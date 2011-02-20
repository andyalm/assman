namespace AlmWitt.Web.ResourceManagement
{
	/// <summary>
	/// Represents an object that can find resources.
	/// </summary>
	public interface IResourceFinder
	{
		/// <summary>
		/// Finds resources of the given file <see cref="ResourceType"/>.
		/// </summary>
		ResourceCollection FindResources(ResourceType resourceType);

		/// <summary>
		/// Finds the resource with the given virtual path.
		/// </summary>
		IResource FindResource(string virtualPath);
	}
}
