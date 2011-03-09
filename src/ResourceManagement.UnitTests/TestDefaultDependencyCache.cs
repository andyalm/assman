using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;

using AlmWitt.Web.ResourceManagement.TestSupport;

using Moq;

using NUnit.Framework;

namespace AlmWitt.Web.ResourceManagement
{
	[TestFixture]
	public class TestDefaultDependencyCache
	{
		private HttpDependencyCache _cache;
		private IDictionary _httpItems;
		private Mock<HttpContextBase> _httpContext;
		private StubResource _resource;
		private string[] _dependencies;

		[SetUp]
		public void SetupContext()
		{
			_httpItems = new Hashtable();
			_httpContext = new Mock<HttpContextBase>();
			_httpContext.SetupGet(c => c.Items).Returns(_httpItems);
			
			_cache = new HttpDependencyCache(() => _httpContext.Object);

			_resource = StubResource.WithPath("~/scripts/myscript.js");
			_resource.LastModified = DateTime.Now.AddDays(-1);
			_dependencies = new string[] { "~/scripts/jquery.js", "~/scripts/site.js" };
			_cache.StoreDependencies(_resource, _dependencies);
		}

		[Test]
		public void WhenResourceDependenciesAreStored_TheyCanBeRetievedByVirtualPath()
		{
			IEnumerable<string> cachedDependencies;
			_cache.TryGetDependencies(_resource.VirtualPath, out cachedDependencies).ShouldBeTrue();
			cachedDependencies.ShouldContainAll(_dependencies);
		}

		[Test]
		public void WhenResourceDependenciesAreStored_TheyCanBeRetievedByResourceInstance()
		{
			IEnumerable<string> cachedDependencies;
			_cache.TryGetDependencies(_resource, out cachedDependencies).ShouldBeTrue();
			cachedDependencies.ShouldContainAll(_dependencies);
		}

		[Test]
		public void WhenResourceDependenciesAreStoredByVirtualPath_TheyArePutInLongRunningCacheWithNoExpiration()
		{
			_cache.StoreDependencies(_resource.VirtualPath, _dependencies);
			_resource.LastModified = DateTime.Now;
			_httpItems.Clear();
			IEnumerable<string> cachedDependencies;
			_cache.TryGetDependencies(_resource.VirtualPath, out cachedDependencies).ShouldBeTrue();
			cachedDependencies.ShouldContainAll(_dependencies);
		}

		[Test]
		public void WhenResourceDependenciesAreNotCachedInHttpButAreCachedInLongRunningCache_DependenciesAreAvailableByResourceOnly()
		{
			_httpItems.Clear();
			IEnumerable<string> cachedDependencies;
			_cache.TryGetDependencies(_resource.VirtualPath, out cachedDependencies).ShouldBeFalse();
			_cache.TryGetDependencies(_resource, out cachedDependencies).ShouldBeTrue();
			cachedDependencies.ShouldContainAll(_dependencies);
		}

		[Test]
		public void WhenResourceDependenciesAreNotCachedInHttpAndAreOutOfDateInLongRunningCache_DependenciesAreNotAvailable()
		{
			_httpItems.Clear();
			_resource.LastModified = DateTime.Now;

			IEnumerable<string> cachedDependencies;
			_cache.TryGetDependencies(_resource, out cachedDependencies).ShouldBeFalse();
		}
	}
}