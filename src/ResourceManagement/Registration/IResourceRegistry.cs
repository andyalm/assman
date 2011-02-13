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
		/// Tries to resolve the given path into a true virtual file path.
		/// </summary>
		/// <param name="path"></param>
		/// <param name="resolvedVirtualPath"></param>
		/// <returns></returns>
		bool TryResolvePath(string path, out string resolvedVirtualPath);

		/// <summary>
		/// Includes the resource at the given path on the current web page.
		/// </summary>
		/// <param name="urlToInclude">The url of the resource to be included.</param>
		void IncludePath(string urlToInclude);

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
