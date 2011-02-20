using System;
using System.IO;

using AlmWitt.Web.ResourceManagement.Configuration;

namespace AlmWitt.Web.ResourceManagement.Registration
{
	public class DependencyResolvingResourceRegistry : IResourceRegistry
	{
		private readonly IResourceRegistry _inner;
		private readonly ResourceManagementContext _context;

		public DependencyResolvingResourceRegistry(IResourceRegistry inner, ResourceManagementContext context)
		{
			_inner = inner;
			_context = context;
		}

		public bool TryResolvePath(string path, out string resolvedVirtualPath)
		{
			return _inner.TryResolvePath(path, out resolvedVirtualPath);
		}

		public void IncludePath(string urlToInclude)
		{
			foreach(var dependency in _context.GetResourceDependencies(urlToInclude))
			{
				_inner.IncludePath(dependency);
			}
			_inner.IncludePath(urlToInclude);
		}

		public void RegisterInlineBlock(Action<TextWriter> block, object key)
		{
			_inner.RegisterInlineBlock(block, key);
		}

		public bool IsInlineBlockRegistered(object key)
		{
			return _inner.IsInlineBlockRegistered(key);
		}
	}
}