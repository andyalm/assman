using System;
using System.IO;

namespace Assman.Registration
{
	/// <summary>
	/// Represents an object that manages the inclusion of resources.
	/// </summary>
	public interface IResourceRegistry
	{
	    /// <summary>
		/// Registers the fact that the given resource is required on the current page.
		/// </summary>
		/// <param name="resourcePath">The virtual path of the resource to be included.</param>
		void Require(string resourcePath);

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
