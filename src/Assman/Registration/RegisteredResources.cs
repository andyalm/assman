using System;
using System.Collections.Generic;
using System.IO;

namespace Assman.Registration
{
	public class RegisteredResources
	{
		public IList<string> Includes { get; set; }
		public IList<Action<TextWriter>> InlineBlocks { get; set; }
	}
}