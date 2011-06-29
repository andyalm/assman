using System;

using Assman.Registration;

namespace Assman.WebForms.Registration
{
	/// <summary>
	/// Includes a client script file on a page.
	/// </summary>
	public class RequireScript : WebResourceIncludeControl
	{
		protected override IResourceRegistry GetRegistryWithName(string name)
		{
			return ResourceRegistries.NamedScriptRegistry(name);
		}
	}
}
