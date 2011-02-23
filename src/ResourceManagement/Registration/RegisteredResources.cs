using System;
using System.Collections.Generic;
using System.IO;

namespace AlmWitt.Web.ResourceManagement.Registration
{
	public class RegisteredResources
	{
		public IList<string> Includes { get; set; }
		public IList<Action<TextWriter>> InlineBlocks { get; set; }
	}
}