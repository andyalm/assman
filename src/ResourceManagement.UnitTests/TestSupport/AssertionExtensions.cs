using NUnit.Framework;

namespace AlmWitt.Web.ResourceManagement.UnitTests.TestSupport
{
	public static class AssertionExtensions
	{
		public static void ShouldEqual<T>(this T actual, T expected)
		{
			Assert.That(actual, Is.EqualTo(expected));
		}

		public static void ShouldBeSameAs<T>(this T actual, T expected)
		{
			Assert.That(actual, Is.SameAs(expected));
		}
		
		public static void ShouldNotBeNull(this object obj)
		{
			Assert.That(obj, Is.Not.Null);
		}

		public static T ShouldBeInstanceOf<T>(this object obj)
		{
			Assert.That(obj, Is.InstanceOf<T>());

			return (T) obj;
		}
	}
}