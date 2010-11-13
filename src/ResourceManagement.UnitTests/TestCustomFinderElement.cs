using System;
using System.Configuration;
using System.Text;

using AlmWitt.Web.ResourceManagement.Configuration;
using AlmWitt.Web.ResourceManagement.TestSupport;

using NUnit.Framework;

namespace AlmWitt.Web.ResourceManagement
{
	[TestFixture]
	public class TestCustomFinderElement
	{
		private CustomFinderElement _element;

		[SetUp]
		public void Init()
		{
			_element = new CustomFinderElement();
		}

		[Test]
		public void WhenTypeSpecified_ItIsNewedUpAndReturned()
		{
			_element.Type = typeof (StubResourceFinder).AssemblyQualifiedName;

			var finder = _element.CreateFinder();

			Assert.That(finder, Is.InstanceOf<StubResourceFinder>());
		}

		[Test]
		[ExpectedException(typeof(ConfigurationErrorsException))]
		public void WhenTypeIsInvalid_ConfigurationErrorsExceptionThrown()
		{
			_element.Type = "bogus";

			_element.CreateFinder();
		}

		[Test]
		[ExpectedException(typeof(ConfigurationErrorsException))]
		public void WhenTypeDoesNotImplementIResourceFinder_ConfigurationErrorsExceptionThrown()
		{
			_element.Type = typeof(StringBuilder).FullName;

			_element.CreateFinder();
		}

		[Test]
		public void WhenFactorySpecified_ItIsUsedToCreateFinderInstance()
		{
			_element.Factory = typeof (StubResourceFinderFactory).AssemblyQualifiedName;

			var finder = _element.CreateFinder();

			Assert.That(finder, Is.InstanceOf<StubResourceFinder>());
		}

		[Test]
		[ExpectedException(typeof(ConfigurationErrorsException))]
		public void WhenFactoryTypeIsInvalid_ConfigurationErrorsExceptionThrown()
		{
			_element.Factory = "bogus";

			_element.CreateFinder();
		}

		[Test]
		[ExpectedException(typeof(ConfigurationErrorsException))]
		public void WhenFactoryDoesNotImplementIResourceFinderFactory_ConfigurationErrorsExceptionThrown()
		{
			_element.Factory = typeof(StringBuilder).FullName;

			_element.CreateFinder();
		}
	}

	
}