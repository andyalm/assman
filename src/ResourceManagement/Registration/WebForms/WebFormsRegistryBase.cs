using System;
using System.IO;
using System.Reflection;
using System.Web.UI;

namespace AlmWitt.Web.ResourceManagement.WebForms
{
	internal abstract class WebFormsRegistryBase : IResourceRegistry
	{
		private Control _control;

		public WebFormsRegistryBase(Control control)
		{
			_control = control;
		}

		public string ResolveUrl(string virtualPath)
		{
			return _control.ResolveUrl(virtualPath);
		}

		public abstract void IncludeUrl(string urlToInclude);

		public string GetEmbeddedResourceUrl(string assemblyName, string resourceName)
		{
			return Page.ClientScript.GetWebResourceUrl(Assembly.Load(assemblyName).GetTypes()[0], resourceName);
		}

		public abstract void RegisterInlineBlock(Action<TextWriter> block, object key);

		public abstract bool IsInlineBlockRegistered(object key);

		protected Control Control
		{
			get { return _control; }
		}

		protected Page Page
		{
			get { return _control.Page; }
		}
	}
}