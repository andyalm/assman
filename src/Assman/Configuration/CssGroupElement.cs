using System;

namespace Assman.Configuration
{
	/// <summary>
	/// A <see cref="ResourceGroupElement"/> used to configure css.
	/// </summary>
	public class CssGroupElement : ResourceGroupElement
	{
		public override ResourceType ResourceType
		{
			get { return ResourceType.Css; }
		}
	}
}
