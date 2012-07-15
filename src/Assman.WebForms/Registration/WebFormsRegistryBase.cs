using System;
using System.IO;
using System.Web.UI;

using Assman.Registration;

namespace Assman.WebForms.Registration
{
	internal abstract class WebFormsRegistryBase : IResourceRegistry
	{
		private readonly Control _control;

		protected WebFormsRegistryBase(Control control)
		{
			_control = control;
		}

	    public abstract void Require(string resourcePath);

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