using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;

namespace Assman.Registration.WebForms
{
	internal abstract class WebFormsRegistryBase : IResourceRegistry
	{
		private readonly Control _control;

		protected WebFormsRegistryBase(Control control)
		{
			_control = control;
		}

		public bool TryResolvePath(string path, out IEnumerable<string> resolvedVirtualPaths)
		{
			resolvedVirtualPaths = new [] {path};
			return true;
		}

		public abstract void IncludePath(string urlToInclude);

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