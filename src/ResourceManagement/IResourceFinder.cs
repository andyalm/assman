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
	}
}
