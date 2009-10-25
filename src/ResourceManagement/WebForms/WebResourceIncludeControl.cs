using System;
using System.Web.UI;

using AlmWitt.Web.ResourceManagement;

namespace AlmWitt.Web.ResourceManagement.WebForms
{
	/// <summary>
	/// Represents a control that includes a web resource.
	/// </summary>
	public abstract class WebResourceIncludeControl : Control
	{
		private string _assemblyName;
		private string _resourceName;
		private WebResourceManager _resourceMgr;
		private string _resourceUrl;

		/// <summary>
		/// Gets or sets the name of the assembly that an embedded resource is embedded in.
		/// </summary>
		public string AssemblyName
		{
			get { return _assemblyName; }
			set { _assemblyName = value; }
		}

		/// <summary>
		/// Gets or sets the name of an embedded resource.
		/// </summary>
		public string ResourceName
		{
			get { return _resourceName; }
			set { _resourceName = value; }
		}

		/// <summary>
		/// Gets or sets the url of the resource.
		/// </summary>
		protected string ResourceUrl
		{
			get { return _resourceUrl; }
			set { _resourceUrl = value; }
		}

		///<summary>
		///Raises the <see cref="E:System.Web.UI.Control.PreRender"></see> event.
		///</summary>
		///
		///<param name="e">An <see cref="T:System.EventArgs"></see> object that contains the event data. </param>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			
			if(String.IsNullOrEmpty(_assemblyName))
			{
				RegisterUrl();
			}
			else
			{
				RegisterEmbeddedResource();
			}
		}

		/// <summary>
		/// When overridden in the inheriting class, gets the type of resource the control will register on the page.
		/// </summary>
		protected abstract ResourceType ResourceType { get; }

		private void RegisterUrl()
		{
			ResourceManager.IncludeUrl(_resourceUrl);
		}

		private void RegisterEmbeddedResource()
		{
			ResourceManager.IncludeEmbeddedResource(_assemblyName, _resourceName);
		}

		/// <summary>
		/// Gets an instance of the <see cref="WebResourceManager"/> object.
		/// </summary>
		protected WebResourceManager ResourceManager
		{
			get
			{
				if(_resourceMgr == null)
				{
					_resourceMgr = WebResourceManager.ForWebForms(this.ResourceType, this);
				}

				return _resourceMgr;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		protected override void Render(HtmlTextWriter writer)
		{
			//nothing to render
		}
	}
}
