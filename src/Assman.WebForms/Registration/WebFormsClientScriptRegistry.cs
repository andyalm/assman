using System;
using System.IO;
using System.Web.UI;

using Assman.Registration;

namespace Assman.WebForms.Registration
{
	internal class WebFormsClientScriptRegistry : WebFormsRegistryBase
	{
		private static readonly Type _type = typeof(Page);

		public WebFormsClientScriptRegistry(Control control) : base(control)
		{
		}

		public override void Require(string resourcePath)
		{
			var resolvedUrl = Control.ResolveUrl(resourcePath);
			Page.ClientScript.RegisterClientScriptInclude(typeof(WebFormsClientScriptRegistry), resourcePath, resolvedUrl);
		}

		public override void RegisterInlineBlock(Action<TextWriter> block, object key)
		{
			var blockAsString = block.RenderToString();
			string stringKey = blockAsString;
			if (key != null)
				stringKey = key.ToString();

			Page.ClientScript.RegisterStartupScript(_type, stringKey, blockAsString);
		}

		public override bool IsInlineBlockRegistered(object key)
		{
			if (key == null)
				throw new ArgumentNullException("key");
			return Page.ClientScript.IsStartupScriptRegistered(_type, key.ToString());
		}
	}
}