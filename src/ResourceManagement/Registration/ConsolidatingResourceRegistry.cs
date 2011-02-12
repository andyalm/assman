using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

using AlmWitt.Web.ResourceManagement.Configuration;

namespace AlmWitt.Web.ResourceManagement.Registration
{
	/// <summary>
	/// Represents a <see cref="IResourceRegistry"/> that consolidates script or css includes that
	/// are registered through it.
	/// </summary>
	/// <remarks>
	/// This is a proxy for an inner <see cref="IResourceRegistry"/> which means it can wrap any implementation of a
	/// <see cref="IResourceRegistry"/>.
	/// </remarks>
	public class ConsolidatingResourceRegistry : IReadableResourceRegistry, IScriptRegistry, IStyleRegistry
	{
		/// <summary>
		/// Wraps the given <see cref="IResourceRegistry"/> with a proxy that ensures that all script includes are consolidated
		/// based on the current <see cref="ResourceManagementConfiguration"/>.
		/// </summary>
		public static IResourceRegistry UseScriptConsolidation(IResourceRegistry scriptRegistry)
		{
			ResourceManagementConfiguration config = ResourceManagementConfiguration.Current;
			if (config.Consolidate && config.ClientScripts.Consolidate)
			{
				scriptRegistry = new ConsolidatingResourceRegistry(scriptRegistry, config, config.GetScriptUrl);
			}

			return scriptRegistry;
		}

		/// <summary>
		/// Wraps the given <see cref="IResourceRegistry"/> with a proxy that ensures that all css includes are consolidated
		/// based on the current <see cref="ResourceManagementConfiguration"/>.
		/// </summary>
		public static IResourceRegistry UseCssConsolidation(IResourceRegistry cssRegistry)
		{
			ResourceManagementConfiguration config = ResourceManagementConfiguration.Current;
			if (config.Consolidate && config.CssFiles.Consolidate)
			{
				cssRegistry = new ConsolidatingResourceRegistry(cssRegistry, config, config.GetStylesheetUrl);
			}

			return cssRegistry;
		}

		private readonly IResourceRegistry _inner;
		private readonly ResourceManagementConfiguration _config;
		private readonly Func<string, string> _getResourceUrl;
		private static readonly Dictionary<string, string> _resolvedUrls = new Dictionary<string, string>();

		internal ConsolidatingResourceRegistry(IResourceRegistry inner, ResourceManagementConfiguration config, Func<string, string> getResourceUrl)
		{
			_inner = inner;
			_config = config;

			Func<string, string> resolveFromCache = virtualPath =>
			{
				if (!_resolvedUrls.ContainsKey(virtualPath))
				{
					_resolvedUrls[virtualPath] = getResourceUrl(virtualPath);
				}
				return _resolvedUrls[virtualPath];
			};
			_getResourceUrl = resolveFromCache;
		}

		public IResourceRegistry Inner
		{
			get { return _inner; }
		}

		public void IncludeUrl(string virtualPath)
		{
			virtualPath = ToCanonicalUrl(virtualPath);
			virtualPath = _getResourceUrl(virtualPath);

			_inner.IncludeUrl(virtualPath);
		}

		public string GetEmbeddedResourceUrl(string assemblyName, string resourceName)
		{
			string shortAssemblyName = assemblyName.ToShortAssemblyName();
			string virtualPath = EmbeddedResource.GetVirtualPath(shortAssemblyName, resourceName);
			string consolidatedUrl = _getResourceUrl(virtualPath);

			if (_config.Assemblies.Contains(shortAssemblyName) && !consolidatedUrl.Equals(virtualPath, StringComparison.OrdinalIgnoreCase))
				return consolidatedUrl;
			else
				return null;
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

		//HACK: This method is internal because it is used by the WebResourceManager.
		//This smells like we need to refactor this a little.
		internal static string ToCanonicalUrl(string url)
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