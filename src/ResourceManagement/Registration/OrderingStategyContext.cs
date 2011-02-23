using System;
using System.Web;

namespace AlmWitt.Web.ResourceManagement.Registration
{
	public class OrderingStategyContext
	{
		private readonly string _currentRegistryName;
		private readonly Func<string, IResourceRegistry> _getNamedRegistry;
		private readonly HttpContextBase _httpContext;

		public OrderingStategyContext(string currentRegistryName, Func<string, IResourceRegistry> getNamedRegistry, HttpContextBase httpContext)
		{
			_currentRegistryName = currentRegistryName;
			_getNamedRegistry = getNamedRegistry;
			_httpContext = httpContext;
		}

		public string CurrentRegistryName
		{
			get { return _currentRegistryName; }
		}

		public IResourceRegistry CurrentRegistry
		{
			get { return _getNamedRegistry(_currentRegistryName); }
		}

		public IResourceRegistry GetRegistry(string name)
		{
			return _getNamedRegistry(name);
		}

		public HttpContextBase HttpContext
		{
			get { return _httpContext; }
		}
	}
}