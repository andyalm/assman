using NUnit.Framework;

using AlmWitt.Web.ResourceManagement.TestSupport;

namespace AlmWitt.Web.ResourceManagement.Registration
{
	[TestFixture]
	public class TestConsolidatingResourceRegistryAccessor
	{
		private GenericResourceRegistryAccessor _innerAccessor;
		private IResourceRegistryAccessor _consolidatingAccessor;

		[SetUp]
		public void SetupContext()
		{
			_innerAccessor = new GenericResourceRegistryAccessor();
			_consolidatingAccessor = _innerAccessor.UseConsolidation();
		}

		[Test]
		public void WhenGettingInstanceOfScriptRegistry_ItsWrappedInAConsolidatingDecorator()
		{
			if(_innerAccessor.ScriptRegistry is ConsolidatingResourceRegistry)
				Assert.Inconclusive();
			
			var scriptRegistry = _consolidatingAccessor.ScriptRegistry;
			scriptRegistry.ShouldBeInstanceOf<ConsolidatingResourceRegistry>();
		}

		[Test]
		public void InstanceOfConsolidatingRegistryIsReused()
		{
			var scriptRegistry = _consolidatingAccessor.ScriptRegistry;
			_consolidatingAccessor.ScriptRegistry.ShouldBeSameAs(scriptRegistry);
		}

		[Test]
		public void WhenGettingInstanceOfNamedScriptRegistry_ItsWrappedInAConsolidatingDecorator()
		{
			const string name = "head";
			
			if (_innerAccessor.NamedScriptRegistry(name) is ConsolidatingResourceRegistry)
				Assert.Inconclusive();

			var scriptRegistry = _consolidatingAccessor.NamedScriptRegistry(name);
			scriptRegistry.ShouldBeInstanceOf<ConsolidatingResourceRegistry>();
		}

		[Test]
		public void InstanceOfNamedConsolidatingRegistryIsReused()
		{
			const string name = "head";
			
			var scriptRegistry = _consolidatingAccessor.NamedScriptRegistry(name);
			_consolidatingAccessor.NamedScriptRegistry(name).ShouldBeSameAs(scriptRegistry);
		}
	}
}