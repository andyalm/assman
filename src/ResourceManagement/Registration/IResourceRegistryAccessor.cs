namespace AlmWitt.Web.ResourceManagement
{
	public interface IResourceRegistryAccessor
	{
		IScriptRegistry ScriptRegistry { get; }

		IScriptRegistry NamedScriptRegistry(string name);

		IStyleRegistry StyleRegistry { get; }

		IStyleRegistry NamedStyleRegistry(string name);
	}
}