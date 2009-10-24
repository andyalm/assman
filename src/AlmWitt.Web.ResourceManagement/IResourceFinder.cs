namespace AlmWitt.Web.ResourceManagement
{
    /// <summary>
    /// Represents an object that can find resources.
    /// </summary>
    public interface IResourceFinder
    {
        /// <summary>
        /// Finds resources with the given file extension.
        /// </summary>
        /// <param name="extension">The file extension beginning with a '.'</param>
        /// <returns></returns>
        ResourceCollection FindResources(string extension);
    }
}