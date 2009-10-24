using System;
using System.Reflection;
using System.Web.UI;

namespace AlmWitt.Web.ResourceManagement.WebForms
{
	internal abstract class WebFormsIncluderBase : IResourceIncluder
	{
		private Control _control;

		public WebFormsIncluderBase(Control control)
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