namespace AlmWitt.Web.ResourceManagement
{
	/// <summary>
	/// Represents a type that creates <see cref="IResourceIncluder"/> objects.
	/// </summary>
	public interface IResourceIncluderFactory
	{
		/// <summary>
		/// Creates a <see cref="IResourceIncluder"/> for including css resources.
		/// </summary>
		/// <returns>An instance of a <see cref="IResourceIncluder"/> that manages the inclusion of css resources.</returns>
		IResourceIncluder CreateCssIncluder();

		/// <summary>
		/// Creates a <see cref="IResourceIncluder"/> for including client script resources.
		/// </summary>
		/// <returns>An instance of a <see cref="IResourceIncluder"/> that manages the inclusion of client script resources.</returns>
		IResourceIncluder CreateClientScriptIncluder();
	}
}