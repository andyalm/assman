using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Assman.Registration
{
	/// <summary>
	/// Represents a <see cref="IResourceRegistry"/> that consolidates script or css includes that
	/// are registered through it.
	/// </summary>
	/// <remarks>
	/// This is a proxy for an inner <see cref="IResourceRegistry"/> which means it can wrap any implementation of a
	/// <see cref="IResourceRegistry"/>.
	/// </remarks>
	public class ConsolidatingResourceRegistry : IReadableResourceRegistry
	{
		private readonly IResourceRegistry _inner;
		private readonly Func<string, IEnumerable<string>> _getResourceUrls;

		internal ConsolidatingResourceRegistry(IResourceRegistry inner, Func<string, IEnumerable<string>> getResourceUrls)
		{
			_inner = inner;
			_getResourceUrls = getResourceUrls;
		}

		public IResourceRegistry Inner
		{
			get { return _inner; }
		}

		public bool TryResolvePath(string path, out IEnumerable<string> resolvedVirtualPaths)
		{
			var resolvedPaths = _getResourceUrls(path);
			if(resolvedPaths.HasAtLeast(2) || !resolvedPaths.Single().Equals(path,  StringComparison.OrdinalIgnoreCase))
			{
				resolvedVirtualPaths = resolvedPaths;
				return true;
			}
			else
			{
				resolvedVirtualPaths = null;
				return false;
			}
		}

		public void IncludePath(string virtualPath)
		{
			virtualPath = ToCanonicalUrl(virtualPath);
			foreach(var pathToInclude in _getResourceUrls(virtualPath))
			{
                _inner.IncludePath(pathToInclude);
			}
		}

		public void RegisterInlineBlock(Action<TextWriter> block, object key)
		{
			_inner.RegisterInlineBlock(block, key);
		}

		public bool IsInlineBlockRegistered(object key)
		{
			return _inner.IsInlineBlockRegistered(key);
		}

		public IEnumerable<string> GetIncludes()
		{
			return _inner.AsReadable().GetIncludes();
		}

		public IEnumerable<Action<TextWriter>> GetInlineBlocks()
		{
			return _inner.AsReadable().GetInlineBlocks();
		}

		private string ToCanonicalUrl(string url)
		{
			//if its a full url, no need to do anything more, just return it.
			if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
				return url;

			if (!url.Contains("?"))
			{
				return VirtualPathUtility.ToAppRelative(url);
			}
			else
			{
				//VirtualPathUtility throws if a query exists in the url.  Strip it
				//off before calling and then append it back on afterwards.
				string[] urlParts = url.Split('?');
				string appRelativeUrl = VirtualPathUtility.ToAppRelative(urlParts[0]);

				return appRelativeUrl + "?" + urlParts[1];
			}
		}
	}
}