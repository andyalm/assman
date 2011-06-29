using System;
using System.IO;
using System.Web.UI;

using Assman.Registration;

namespace Assman.WebForms.Registration
{
	internal class WebFormsStyleRegistry : WebFormsRegistryBase
	{
		const string LinkTemplate = "<link type=\"text/css\" rel=\"Stylesheet\" href=\"{0}\" />";
		
		public WebFormsStyleRegistry(Control control) : base(control) { }

		public override void Require(string resourcePath)
		{
			var linkElement = String.Format(LinkTemplate, resourcePath);
			AddToHeader(resourcePath, linkElement);
		}

		public override void RegisterInlineBlock(Action<TextWriter> block, object key)
		{
			var blockAsString = block.RenderToString();
			AddToHeader(blockAsString, blockAsString);
		}

		private void AddToHeader(string key, string content)
		{
			if(Page.Header != null)
			{
				Page.PreRenderComplete += delegate
				{
					Page.Header.Controls.Add(new LiteralControl(content));
				};
			}
			else
			{
				Page.ClientScript.RegisterClientScriptBlock(typeof(Page), key, content, false);
			}
		}

		public override bool IsInlineBlockRegistered(object key)
		{
			throw new NotImplementedException();
		}
	}
}