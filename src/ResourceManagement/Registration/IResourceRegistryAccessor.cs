namespace AlmWitt.Web.ResourceManagement
{
	public interface IResourceRegistryAccessor
	{
		IResourceRegistry ScriptRegistry { get; }

		IResourceRegistry NamedScriptRegistry(string name);

		IResourceRegistry StyleRegistry { get; }

		IResourceRegistry NamedStyleRegistry(string name);
	}
}