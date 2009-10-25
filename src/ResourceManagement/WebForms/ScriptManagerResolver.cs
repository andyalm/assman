using System;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.Adapters;

using AlmWitt.Web.ResourceManagement;

namespace AlmWitt.Web.ResourceManagement.WebForms
{
	/// <summary>
	/// Ensures all scripts that are registered via the ScriptManager get put through
	/// the ResourceManagement library.
	/// </summary>
	public class ScriptManagerResolver : ControlAdapter
	{
		/// <summary>
		/// Creates an instance of a <see cref="ScriptManagerResolver"/> that can be used for resolving <see cref="ScriptReference"/> objects.
		/// </summary>
		/// <param name="control">A control that lives on the web page (or the page control itself).</param>
		/// <returns></returns>
		public static ScriptManagerResolver GetInstance(Control control)
		{
			return new ScriptManagerResolver(WebResourceManager.ForWebForms(ResourceType.ClientScript, control));
		}
		
		private string[] msAjaxScripts = new string[] { "MicrosoftAjax.js", "MicrosoftAjaxWebForms.js", "MicrosoftAjaxTimer.js" };
		private const string MicrosoftAjaxAssemblyName = "System.Web.Extensions";
		private WebResourceManager _resourceMgr;

		/// <summary>
		/// Creates an instance of a <see cref="ScriptManagerResolver"/>.  This constructor is only intended to be used by the ASP.NET
		/// infrastructure when configured as a <see cref="ControlAdapter"/>.
		/// </summary>
		public ScriptManagerResolver()
		{
		}

		/// <summary>
		/// Creates a new instance of a <see cref="ScriptManagerResolver"/>.
		/// </summary>
		/// <param name="resourceMgr"></param>
		protected ScriptManagerResolver(WebResourceManager resourceMgr)
		{
			_resourceMgr = resourceMgr;
		}

		///<summary>
		///Overrides the <see cref="M:System.Web.UI.Control.OnInit(System.EventArgs)"></see> method for the associated control.
		///</summary>
		///
		///<param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data. </param>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			_resourceMgr = WebResourceManager.ForWebForms(ResourceType.ClientScript, Control);
			Control.ResolveScriptReference += Control_OnResolveScriptReference;
		}

		private void Control_OnResolveScriptReference(object sender, ScriptReferenceEventArgs e)
		{
			ResolveScriptReference(e.Script);
		}

		/// <summary>
		/// Resolves the given <see cref="ScriptReference"/> so that it points to the consolidated resource.
		/// </summary>
		/// <param name="scriptReference"></param>
		public void ResolveScriptReference(ScriptReference scriptReference)
		{
			if (IsMsAjaxScript(scriptReference))
			{
				scriptReference.Assembly = MicrosoftAjaxAssemblyName;
				if (UseMsDebugScripts)
				{
					string baseName = scriptReference.Name.Substring(0, scriptReference.Name.Length - 3);
					scriptReference.Name = baseName + ".debug.js";
				}
			}

			if (!String.IsNullOrEmpty(scriptReference.Assembly))
			{
				string consolidatedUrl = _resourceMgr.GetConsolidatedResourceUrl(scriptReference.Assembly, scriptReference.Name);
				if(!String.IsNullOrEmpty(consolidatedUrl))
				{
					scriptReference.Path = consolidatedUrl;
					scriptReference.Assembly = String.Empty;
					scriptReference.Name = String.Empty;
				}
			}
			else if (!String.IsNullOrEmpty(scriptReference.Path))
			{
				scriptReference.Path = _resourceMgr.GetUrl(scriptReference.Path);
			}
		}

		private bool IsMsAjaxScript(ScriptReference scriptReference)
		{
			return Array.IndexOf(msAjaxScripts, scriptReference.Name) >= 0;
		}

		/// <summary>
		/// Gets whether the debug version of the MS Ajax scripts should be used.
		/// </summary>
		protected virtual bool UseMsDebugScripts
		{
			get
			{
				return Util.IsDebugMode;
			}
		}

		/// <summary>
		/// Gets the instance of the <see cref="ScriptManager"/> control.
		/// </summary>
		protected new ScriptManager Control
		{
			get
			{
				return (ScriptManager)base.Control;
			}
		}
	}
}
