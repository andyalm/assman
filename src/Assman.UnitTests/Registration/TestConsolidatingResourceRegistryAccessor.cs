using System.Linq;

using Assman.Configuration;

using NUnit.Framework;

using Assman.TestSupport;

namespace Assman.Registration
{
	[TestFixture]
	public class TestConsolidatingResourceRegistryAccessor
	{
		private IResourceRegistryAccessor _accessor;
		private AssmanContext _context;
		private StubResourceFinder _finder;

		[SetUp]
		public void SetupContext()
		{
			_finder = new StubResourceFinder();
			
			_context = AssmanContext.Create(ResourceMode.Release);
			_context.ManageDependencies = true;
			_context.AddFinder(_finder);
			_accessor = new ConsolidatingResourceRegistryAccessor(_context);
		}

		[TearDown]
		public void TeardownContext()
		{
			AssmanContext.Current = null;
		}

		[Test]
		public void WhenGettingInstanceOfScriptRegistry_ItsWrappedInAConsolidatingDecorator()
		{
			var scriptRegistry = _accessor.ScriptRegistry;
			scriptRegistry.ShouldBeInstanceOf<ConsolidatingResourceRegistry>();
		}

		[Test]
		public void InstanceOfConsolidatingRegistryIsReused()
		{
			var scriptRegistry = _accessor.ScriptRegistry;
			_accessor.ScriptRegistry.ShouldBeSameAs(scriptRegistry);
		}

		[Test]
		public void WhenGettingInstanceOfNamedScriptRegistry_ItsWrappedInAConsolidatingDecorator()
		{
			const string name = "head";
			
			var scriptRegistry = _accessor.NamedScriptRegistry(name);
			scriptRegistry.ShouldBeInstanceOf<ConsolidatingResourceRegistry>();
		}

		[Test]
		public void InstanceOfNamedConsolidatingRegistryIsReused()
		{
			const string name = "head";
			
			var scriptRegistry = _accessor.NamedScriptRegistry(name);
			_accessor.NamedScriptRegistry(name).ShouldBeSameAs(scriptRegistry);
		}

		[Test]
		public void WhenGettingRegisteredScripts_IncludesAndScriptBlocksAssociatedWithNamedRegistryAreReturned()
		{
			var scriptRegistry = _accessor.NamedScriptRegistry("MyScriptRegistry");
			scriptRegistry.Require("~/myscript.js");
			scriptRegistry.Require("~/myotherscript.js");
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
			scriptRegistry1.Require("~/common.js");
			scriptRegistry1.Require("~/myscript1.js");

			var scriptRegistry2 = _accessor.NamedScriptRegistry("MyScriptRegistry2");
			scriptRegistry2.Require("~/common.js");
			scriptRegistry2.Require("~/myscript2.js");

			var registeredScripts1 = _accessor.GetRegisteredScripts("MyScriptRegistry1");
			registeredScripts1.Includes.CountShouldEqual(2);
			registeredScripts1.Includes[0].ShouldEqual("~/common.js");
			registeredScripts1.Includes[1].ShouldEqual("~/myscript1.js");

			var registeredScripts2 = _accessor.GetRegisteredScripts("MyScriptRegistry2");
			registeredScripts2.Includes.CountShouldEqual(1);
			registeredScripts2.Includes[0].ShouldEqual("~/myscript2.js");
		}

		[Test]
		public void WhenAScriptIsInMoreThanOneGroup_AndOneGroupWasExplicitlyRequired_ThenGroupExplicitlyRequiredGroupIsIncluded()
		{
			var aOnlyFile = _finder.CreateResource("~/a/script.js");
			var bOnlyFile = _finder.CreateResource("~/b/scripts.js");
			var fileInBothGroups = _finder.CreateResource("~/shared/script.js");
			
			var groupA = new ScriptGroupElement();
			groupA.ConsolidatedUrl = "~/groups/groupA.js";
			groupA.Include.AddPath(aOnlyFile.VirtualPath);
			groupA.Include.AddPath(fileInBothGroups.VirtualPath);
			_context.ScriptGroups.Add(groupA);

			var groupB = new ScriptGroupElement();
			groupB.ConsolidatedUrl = "~/groups/groupB.js";
			groupB.Include.AddPath(bOnlyFile.VirtualPath);
			groupB.Include.AddPath(fileInBothGroups.VirtualPath);
			_context.ScriptGroups.Add(groupB);
			
			var scriptRegistry1 = _accessor.ScriptRegistry;
			scriptRegistry1.Require(fileInBothGroups.VirtualPath);
			scriptRegistry1.Require(groupB.ConsolidatedUrl);

			var includedScripts = scriptRegistry1.GetIncludes().ToList();
			includedScripts.CountShouldEqual(1);
			includedScripts[0].ShouldEqual(groupB.ConsolidatedUrl);
		}

	    [Test]
	    public void WhenTwoScriptsMappingToTheSameGroupAreRequiredInTwoSeparateRegistries_TheGroupIsOnlyIncludedInOneRegistry()
	    {
            var scriptA = _finder.CreateResource("~/scriptA.js");
            var scriptB = _finder.CreateResource("~/scriptB.js");

            var group = new ScriptGroupElement();
            group.ConsolidatedUrl = "~/groups.js";
            group.Include.AddPath(scriptA.VirtualPath);
            group.Include.AddPath(scriptB.VirtualPath);
            _context.ScriptGroups.Add(group);

            var scriptRegistry1 = _accessor.ScriptRegistry;
            scriptRegistry1.Require(scriptA.VirtualPath);

	        var scriptRegistry2 = _accessor.NamedScriptRegistry("another");
            scriptRegistry2.Require(scriptB.VirtualPath);

            var includedScripts1 = scriptRegistry1.GetIncludes().ToList();
            includedScripts1.CountShouldEqual(1);
            includedScripts1[0].ShouldEqual(group.ConsolidatedUrl);

            scriptRegistry2.GetIncludes().CountShouldEqual(0);
	    }

        [Test]
        public void WhenAScriptIsRequiredThatIsPartOfAGroupAlreadyClaimed_ItIsIgnored()
        {
            var scriptA = _finder.CreateResource("~/scriptA.js");
            var scriptB = _finder.CreateResource("~/scriptB.js");

            var group = new ScriptGroupElement();
            group.ConsolidatedUrl = "~/groups.js";
            group.Include.AddPath(scriptA.VirtualPath);
            group.Include.AddPath(scriptB.VirtualPath);
            _context.ScriptGroups.Add(group);

            var scriptRegistry1 = _accessor.ScriptRegistry;
            scriptRegistry1.Require(scriptA.VirtualPath);

            var includedScripts1 = scriptRegistry1.GetIncludes().ToList();
            includedScripts1.CountShouldEqual(1);
            includedScripts1[0].ShouldEqual(group.ConsolidatedUrl);

            var scriptRegistry2 = _accessor.NamedScriptRegistry("another");
            scriptRegistry2.Require(scriptB.VirtualPath);

            scriptRegistry2.GetIncludes().CountShouldEqual(0);
        }
	}
}