using AlmWitt.Web.ResourceManagement.TestSupport;

using NUnit.Framework;

namespace AlmWitt.Web.ResourceManagement.Registration
{
	[TestFixture]
	public class TestGenericResourceRegistryAccessor
	{
		private GenericResourceRegistryAccessor _accessor;

		[SetUp]
		public void SetupContext()
		{
			_accessor = new GenericResourceRegistryAccessor();
		}
		
		[Test]
		public void WhenRequestingNamedScriptRegistryForFirstTime_NewOneIsReturned()
		{
			var scriptRegistry = _accessor.NamedScriptRegistry("MyScriptRegistry");

			scriptRegistry.ShouldNotBeNull();
			scriptRegistry.ShouldBeInstanceOf<GenericResourceRegistry>();
		}

		[Test]
		public void WhenRequestingNamedScriptRegistryForTheNthTime_SameInstanceIsReturned()
		{
			var scriptRegistry1 = _accessor.NamedScriptRegistry("MyScriptRegistry");
			var scriptRegistry2 = _accessor.NamedScriptRegistry("MyScriptRegistry");

			scriptRegistry1.ShouldBeSameAs(scriptRegistry2);
		}
	}
}