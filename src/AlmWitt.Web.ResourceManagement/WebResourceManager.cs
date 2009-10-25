using System;
using System.Web;
using System.Web.UI;

using AlmWitt.Web.ResourceManagement.Configuration;
using AlmWitt.Web.ResourceManagement.WebForms;

namespace AlmWitt.Web.ResourceManagement
{
	/// <summary>
	/// Manages web resources.
	/// </summary>
	public class WebResourceManager
	{
		/// <summary>
		/// Creates an instance of a <see cref="WebResourceManager"/> that will manage the inclusion of resources on a web forms <see cref="Page"/>.
		/// </summary>
		/// <param name="resourceType">The <see cref="ResourceType"/> that the <see cref="WebResourceManager"/> instance will be managing.</param>
		/// <param name="control">A <see cref="Control"/> that exists on the web forms <see cref="Page"/>.  Note that this control will be used as the context
		/// by which relative url's are resolved.</param>
		/// <returns></returns>
		public static WebResourceManager ForWebForms(ResourceType resourceType, Control control)
		{
			return new WebResourceManager(resourceType, new WebFormsIncluderFactory(control));
		}

		/// <summary>
		/// Creates an instance of a <see cref="WebResourceManager"/> that will manage the inclusion of resources.
		/// </summary>
		/// <param name="resourceType">The <see cref="ResourceType"/> that the <see cref="WebResourceManager"/> instance will be managing.</param>
		/// <param name="factory">A <see cref="IResourceIncluderFactory"/> used to create <see cref="IResourceIncluder"/>s that will be used
		/// to include the resources on the page.</param>
		/// <returns></returns>
		public static WebResourceManager Create(ResourceType resourceType, IResourceIncluderFactory factory)
		{
			return new WebResourceManager(resourceType, factory);
		}

		private ResourceType _resourceType;
		private IResourceIncluder _includer;

		/// <summary>
		/// Constructs a new instance of a <see cref="WebResourceManager"/>.
		/// </summary>
		/// <param name="resourceType">The <see cref="ResourceType"/> that the <see cref="WebResourceManager"/> instance will be managing.</param>
		/// <param name="factory">A <see cref="IResourceIncluderFactory"/> used to create <see cref="IResourceIncluder"/>s that will be used
		/// to include the resources on the page.</param>
		protected WebResourceManager(ResourceType resourceType, IResourceIncluderFactory factory)
    	{
    		_resourceType = resourceType;
			_includer = resourceType.GetIncluder(factory);
    	}

		/// <summary>
		/// Includes an embedded resource on the page.  If the resource is configured to be consolidated
		/// then the consolidated file that contains the content of this embedded resource will be included instead.
		/// </summary>
		/// <param name="assemblyName">The assembly containing the embedded resource.</param>
		/// <param name="resourceName">The name of the embedded resource.</param>
		public void IncludeEmbeddedResource(string assemblyName, string resourceName)
		{
			string url = GetEmbeddedResourceUrl(assemblyName, resourceName);
			_includer.IncludeUrl(url);
		}

		/// <summary>
		/// Includes an embedded resource on the page.  If the resource is configured to be consolidated
		/// then the consolidated file that contains the content of this embedded resource will be included instead.
		/// </summary>
		/// <param name="type">A type that exists in the same assembly as the embedded resource.</param>
		/// <param name="resourceName">The name of the embedded resource.</param>
		public void IncludeEmbeddedResource(Type type, string resourceName)
		{
    		string url = GetEmbeddedResourceUrl(type, resourceName);
			_includer.IncludeUrl(url);
		}

		/// <summary>
		/// Includes the resource with the given url on the page.  If the resource is configured to be consolidated
		/// then the consolidated file that contains the content of this embedded resource will be included instead.
		/// </summary>
		/// <param name="virtualPath"></param>
		public void IncludeUrl(string virtualPath)
		{
			string resolvedUrl = GetUrl(virtualPath);

			_includer.IncludeUrl(resolvedUrl);
		}

		/// <summary>
		/// Gets the url to a file that includes the contents of the file of the given url.
		/// </summary>
		/// <param name="virtualPath">The virtual path to the file.</param>
		/// <returns></returns>
		public string GetUrl(string virtualPath)
    	{
    		//resolve the url before making it app-relative.
			string resolvedUrl = _includer.ResolveUrl(virtualPath);
    		string appRelativePath = ToAppRelativeUrl(resolvedUrl);
    		//use the app-relative path to match against when figuring out the resource url
    		string resourceUrl = GetConsolidatedUrl(appRelativePath);

    		//re-resolve the resource url as it could be different from the one we already resolved.
			return _includer.ResolveUrl(resourceUrl);
    	}

		/// <summary>
		/// Gets the url to a file that includes the contents of the given embedded resource.
		/// </summary>
		/// <param name="type">A type that exists in the same assembly as the embedded resource.</param>
		/// <param name="resourceName">The name of the embedded resource.</param>
		/// <returns></returns>
		public string GetEmbeddedResourceUrl(Type type, string resourceName)
		{
			return GetEmbeddedResourceUrl(type.Assembly.GetName().Name, resourceName);
		}

		/// <summary>
		/// Gets the url to a file that includes the contents of the given embedded resource.
		/// </summary>
		/// <param name="assemblyName">The assembly containing the embedded resource.</param>
		/// <param name="resourceName">The name of the embedded resource.</param>
		public string GetEmbeddedResourceUrl(string assemblyName, string resourceName)
		{
			string url = GetConsolidatedResourceUrl(assemblyName, resourceName);
			if(String.IsNullOrEmpty(url))
			{
				url = _includer.GetEmbeddedResourceUrl(assemblyName, resourceName);
			}

			return url;
		}

		/// <summary>
		/// If the given embedded resource is configured to be consolidated, then
		/// a url to the consolidated file is returned.  Otherwise, <c>null</c> is returned.
		/// </summary>
		/// <param name="assemblyName"></param>
		/// <param name="resourceName"></param>
		/// <returns></returns>
		internal string GetConsolidatedResourceUrl(string assemblyName, string resourceName)
		{
			string virtualPath = EmbeddedResource.GetVirtualPath(assemblyName, resourceName);
			string consolidatedUrl = GetConsolidatedUrl(virtualPath);

			if (Config.Assemblies.Contains(assemblyName) &&
			    !consolidatedUrl.Equals(virtualPath, StringComparison.OrdinalIgnoreCase))
			{
				return _includer.ResolveUrl(consolidatedUrl);
			}
			else
			{
				return null;
			}
		}

		private string GetConsolidatedUrl(string appRelativeUrl)
		{
			return _resourceType.GetResourceUrl(Config, appRelativeUrl);
		}

		/// <summary>
		/// Converts the given url to an app-relative path (e.g. ~/MyFolder/MyFile.aspx)
		/// </summary>
		/// <param name="relativeUrl">A url to be converted to app-relative form.</param>
		/// <returns></returns>
		protected virtual string ToAppRelativeUrl(string relativeUrl)
		{
			return VirtualPathUtility.ToAppRelative(relativeUrl);
		}

		/// <summary>
		/// The configuration that is used to determine how to consolidate the resources.
		/// </summary>
		protected virtual ResourceManagementConfiguration Config
		{
			get { return ResourceManagementConfiguration.Current; }
		}
	}
}
