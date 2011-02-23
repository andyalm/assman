using System;
using System.Collections.Generic;
using System.IO;

namespace AlmWitt.Web.ResourceManagement.Registration
{
	public interface IReadableResourceRegistry : IResourceRegistry
	{
		IEnumerable<string> GetIncludes();
		IEnumerable<Action<TextWriter>> GetInlineBlocks();
	}
}