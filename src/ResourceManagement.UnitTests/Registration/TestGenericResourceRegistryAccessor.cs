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

		[Test]
		public void WhenGettingRegisteredScripts_IncludesAndScriptBlocksAssociatedWithNamedRegistryAreReturned()
		{
			var scriptRegistry = _accessor.NamedScriptRegistry("MyScriptRegistry");
			scriptRegistry.IncludePath("~/myscript.js");
			scriptRegistry.IncludePath("~/myotherscript.js");
			scriptRegistry.RegisterInlineBlock("alert('do something');");
			scriptRegistry.RegisterInlineBlock("alert('do something else');");

			var registeredScripts = _accessor.GetRegisteredScripts("MyScriptRegistry");
			registeredScripts.Includes[0].ShouldEqual("~/myscript.js");
			registeredScripts.Includes[1].ShouldEqual("~/myotherscript.js");

			registeredScripts.InlineBlocks[0].RenderToString().ShouldEqual("alert('do something');");
			registeredScripts.InlineBlocks[1].RenderToString().ShouldEqual("alert('do something else');");
		}

		[Test]
		public void WhenAScriptFileIsRegisteredInTwoRegistries_ItIsOnlyReturnedAsPartOfTheFirstOneRequested()
		{
			var scriptRegistry1 = _accessor.NamedScriptRegistry("MyScriptRegistry1");
			scriptRegistry1.IncludePath("~/common.js");
			scriptRegistry1.IncludePath("~/myscript1.js");

			var scriptRegistry2 = _accessor.NamedScriptRegistry("MyScriptRegistry2");
			scriptRegistry2.IncludePath("~/common.js");
			scriptRegistry2.IncludePath("~/myscript2.js");

			var registeredScripts1 = _accessor.GetRegisteredScripts("MyScriptRegistry1");
			registeredScripts1.Includes.CountShouldEqual(2);
			registeredScripts1.Includes[0].ShouldEqual("~/common.js");
			registeredScripts1.Includes[1].ShouldEqual("~/myscript1.js");

			var registeredScripts2 = _accessor.GetRegisteredScripts("MyScriptRegistry2");
			registeredScripts2.Includes.CountShouldEqual(1);
			registeredScripts2.Includes[0].ShouldEqual("~/myscript2.js");
		}
	}
}