namespace AlmWitt.Web.ResourceManagement
{
	public class GenericResourceRegistryAccessor : IResourceRegistryAccessor
	{
		private readonly ResourceRegistryMap<IResourceRegistry> _scriptRegistries = new ResourceRegistryMap<IResourceRegistry>(() => new GenericResourceRegistry());
		private readonly ResourceRegistryMap<IResourceRegistry> _styleRegistries = new ResourceRegistryMap<IResourceRegistry>(() => new GenericResourceRegistry());
		
		public IResourceRegistry ScriptRegistry
		{
			get { return _scriptRegistries.GetDefaultRegistry(); }
		}

		public IResourceRegistry NamedScriptRegistry(string name)
		{
			return _scriptRegistries.GetRegistryWithName(name);
		}

		public IResourceRegistry StyleRegistry
		{
			get { return _styleRegistries.GetDefaultRegistry(); }
		}

		public IResourceRegistry NamedStyleRegistry(string name)
		{
			return _styleRegistries.GetRegistryWithName(name);
		}
	}
}