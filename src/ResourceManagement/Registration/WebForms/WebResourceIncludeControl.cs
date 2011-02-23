using System;
using System.Web.UI;

namespace AlmWitt.Web.ResourceManagement.Registration.WebForms
{
	/// <summary>
	/// Represents a control that includes a web resource.
	/// </summary>
	public abstract class WebResourceIncludeControl : Control
	{
		private IResourceRegistryAccessor _resourceRegistries;

		/// <summary>
		/// Gets or sets the name of the assembly that an embedded resource is embedded in.
		/// </summary>
		public string AssemblyName { get; set; }

		/// <summary>
		/// Gets or sets the name of an embedded resource.
		/// </summary>
		public string ResourceName { get; set; }

		/// <summary>
		/// Gets or sets the url of the resource.
		/// </summary>
		protected string ResourceUrl { get; set; }

		/// <summary>
		/// Gets the name of the <see cref="IResourceRegistry"/> that the resource will be included into.
		/// </summary>
		public string RegistryName { get; set; }

		protected WebResourceIncludeControl()
		{
			RegistryName = "Default";
		}

		///<summary>
		///Raises the <see cref="E:System.Web.UI.Control.PreRender"></see> event.
		///</summary>
		///
		///<param name="e">An <see cref="T:System.EventArgs"></see> object that contains the event data. </param>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			var registry = GetRegistryWithName(RegistryName);
			
			if(String.IsNullOrEmpty(AssemblyName))
			{
				registry.IncludePath(ResourceUrl);
			}
			else
			{
				registry.IncludeEmbeddedResource(AssemblyName, ResourceName);
			}
		}

		protected abstract IResourceRegistry GetRegistryWithName(string name);

		protected IResourceRegistryAccessor ResourceRegistries
		{
			get
			{
				if(_resourceRegistries == null)
				{
					_resourceRegistries = WebFormsRegistryAccessor.GetInstance(this);
				}

				return _resourceRegistries;
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
