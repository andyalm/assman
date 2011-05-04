namespace Assman.Registration.WebForms
{
	/// <summary>
	/// Includes a css file on a page.
	/// </summary>
	public class RequireStylesheet : WebResourceIncludeControl
	{
		protected override IResourceRegistry GetRegistryWithName(string name)
		{
			return ResourceRegistries.NamedStyleRegistry(name);
		}
	}
}
