using System;
using System.Collections.Generic;
using System.IO;

namespace Assman.Registration
{
	public interface IReadableResourceRegistry : IResourceRegistry
	{
		IEnumerable<string> GetIncludes();
		IEnumerable<Action<TextWriter>> GetInlineBlocks();
	}
}