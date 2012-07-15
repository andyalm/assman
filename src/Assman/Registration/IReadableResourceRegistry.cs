using System;
using System.Collections.Generic;
using System.IO;

namespace Assman.Registration
{
	//TODO: Make this interface obsolete and collapse its members into IResourceRegistry
	public interface IReadableResourceRegistry : IResourceRegistry
	{
		IEnumerable<string> GetIncludes();
		IEnumerable<Action<TextWriter>> GetInlineBlocks();
	}
}