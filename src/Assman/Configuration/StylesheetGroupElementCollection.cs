

using System;

namespace Assman.Configuration
{
	/// <summary>
	/// Represents a collection of <see cref="StylesheetGroupElement"/>'s.
	/// </summary>
	public class StylesheetGroupElementCollection : ResourceGroupElementCollection
	{
		protected override ResourceGroupElement CreateGroupElement()
		{
			return new StylesheetGroupElement();
		}
	}
}