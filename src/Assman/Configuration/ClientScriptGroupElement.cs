using System;

namespace Assman.Configuration
{
	/// <summary>
	/// A <see cref="ResourceGroupElement"/> used to configure client scripts.
	/// </summary>
	public class ClientScriptGroupElement : ResourceGroupElement
	{
		public override ResourceType ResourceType
		{
			get { return ResourceType.ClientScript; }
		}
	}


}
