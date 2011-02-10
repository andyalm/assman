namespace AlmWitt.Web.ResourceManagement
{
	public class GenericResourceRegistryAccessor : IResourceRegistryAccessor
	{
		private readonly ResourceRegistryMap<IScriptRegistry> _scriptRegistries = new ResourceRegistryMap<IScriptRegistry>(() => new GenericResourceRegistry());
		private readonly ResourceRegistryMap<IStyleRegistry> _styleRegistries = new ResourceRegistryMap<IStyleRegistry>(() => new GenericResourceRegistry());
		
		public IScriptRegistry ScriptRegistry
		{
			get { return _scriptRegistries.GetDefaultRegistry(); }
		}

		public IScriptRegistry NamedScriptRegistry(string name)
		{
			return _scriptRegistries.GetRegistryWithName(name);
		}

		public IStyleRegistry StyleRegistry
		{
			get { return _styleRegistries.GetDefaultRegistry(); }
		}

		public IStyleRegistry NamedStyleRegistry(string name)
		{
			return _styleRegistries.GetRegistryWithName(name);
		}
	}
}