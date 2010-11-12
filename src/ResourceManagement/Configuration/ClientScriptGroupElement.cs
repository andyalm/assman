using System;

namespace AlmWitt.Web.ResourceManagement.Configuration
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
