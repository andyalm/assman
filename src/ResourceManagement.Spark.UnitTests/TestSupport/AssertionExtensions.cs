using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using NUnit.Framework;

namespace AlmWitt.Web.ResourceManagement.Spark.TestSupport
{
    public static class AssertionExtensions
    {
        public static void ShouldEqual<T>(this T actual, T expected)
        {
            Assert.AreEqual(expected, actual);
        }

        public static void ShouldNotEqual<T>(this T actual, T expected)
        {
            Assert.AreNotEqual(expected, actual);
        }

        public static void ShouldMatch(this string actual, string regexExpression)
        {
            actual.ShouldMatch(regexExpression, RegexOptions.None);
        }

        public static void ShouldMatch(this string actual, string regexExpression, RegexOptions options)
        {
            Assert.IsTrue(Regex.IsMatch(actual, regexExpression, options), "The string '" + actual + "' does not match the regex expression '" + regexExpression + "'.");
        }

        public static void ShouldBeNull(this object obj)
        {
            Assert.IsNull(obj);
        }

        public static void ShouldNotBeNull(this object obj)
        {
            Assert.IsNotNull(obj);
        }

        public static T ShouldBeInstanceOfType<T>(this object obj)
        {
            return obj.ShouldBeInstanceOfType<T>("Expected the object of type '" + obj.GetType().FullName +
                                              "' to be assignable to type '" + typeof(T).FullName + "'.");
        }

        public static T ShouldBeInstanceOfType<T>(this object obj, string message)
        {
            obj.ShouldNotBeNull();
            Assert.IsInstanceOfType(typeof(T), obj, message);

            return (T)obj;
        }

        public static void ShouldContain<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
        {
            collection.Any<T>(predicate).ShouldEqual(true);
        }
    }
}