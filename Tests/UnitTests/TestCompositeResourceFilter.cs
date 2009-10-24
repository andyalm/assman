using System;

using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace AlmWitt.Web.ResourceManagement.UnitTests
{
	[TestFixture]
	public class TestCompositeResourceFilter
	{
		private CompositeResourceFilter _instance;

        [SetUp]
        public void Init()
        {
            _instance = new CompositeResourceFilter();
        }
		
		[Test]
		public void TwoTruesReturnsTrue()
		{
			_instance.AddFilter(ResourceFilters.True);
			_instance.AddFilter(ResourceFilters.True);

			Assert.That(_instance.IsMatch(null), Is.True);
		}

		[Test]
		public void TwoFalseReturnsFalse()
		{
			_instance.AddFilter(ResourceFilters.False);
			_instance.AddFilter(ResourceFilters.False);

			Assert.That(_instance.IsMatch(null), Is.False);
		}

		[Test]
		public void TrueAndFalseReturnsTrue()
		{
			_instance.AddFilter(ResourceFilters.True);
			_instance.AddFilter(ResourceFilters.False);

			Assert.That(_instance.IsMatch(null), Is.True);
		}

		[Test]
		public void NoFilterReturnsFalse()
		{
			Assert.That(_instance.IsMatch(null), Is.False);
		}
	}
}