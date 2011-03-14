using System;
using System.IO;
using System.Web.UI;

namespace AlmWitt.Web.ResourceManagement.Registration.WebForms
{
	internal class WebFormsCssRegistry : WebFormsRegistryBase
	{
		const string LinkTemplate = "<link type=\"text/css\" rel=\"Stylesheet\" href=\"{0}\" />";
		
		public WebFormsCssRegistry(Control control) : base(control) { }

		public override void IncludePath(string urlToInclude)
		{
			var linkElement = String.Format(LinkTemplate, urlToInclude);
			AddToHeader(urlToInclude, linkElement);
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