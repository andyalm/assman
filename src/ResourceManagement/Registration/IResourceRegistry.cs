using System;
using System.IO;

namespace AlmWitt.Web.ResourceManagement
{
	/// <summary>
	/// Represents an object that manages the inclusion of resources.
	/// </summary>
	public interface IResourceRegistry
	{
		/// <summary>
		/// Resolves a virtual or relative url to one that is usable by the web browser.
		/// </summary>
		/// <param name="virtualPath">A virtual or relative url.</param>
		/// <returns></returns>
		//string ResolveUrl(string virtualPath);
		//TODO: maybe later???
		
		/// <summary>
		/// Gets a url that will return the contents of the specified embedded resource.
		/// </summary>
		/// <param name="assemblyName">The name of the assembly that the resource is embedded in.</param>
		/// <param name="resourceName">The name of the embedded resource.</param>
		/// <returns></returns>
		string GetEmbeddedResourceUrl(string assemblyName, string resourceName);
		
		/// <summary>
		/// Includes the resource on the current web page.
		/// </summary>
		/// <param name="urlToInclude">The url of the resource to be included.</param>
		void IncludeUrl(string urlToInclude);

		/// <summary>
		/// Registers an inline block that will appear inline on the page directly below the includes of this <see cref="IResourceRegistry"/>.
		/// </summary>
		/// <param name="block">The delegate that will write some inline css or javascript that will appear on the page.</param>
		/// <param name="key">A unique key used to identify the inline block.  This is optional and can be set to <c>null</c>.</param>
		void RegisterInlineBlock(Action<TextWriter> block, object key);

		/// <summary>
		/// Indicates that an inline block with the given key has been registered.
		/// </summary>
		bool IsInlineBlockRegistered(object key);
	}
}
