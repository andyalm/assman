using System;
using System.Collections.Generic;
using System.IO;

namespace AlmWitt.Web.ResourceManagement.Registration
{
	/// <summary>
	/// An implementation of <see cref="IResourceRegistry"/> that doesn't do anything. (null object pattern)
	/// </summary>
	public class NullResourceRegistry : IReadableResourceRegistry
	{
		private static readonly IReadableResourceRegistry _instance = new NullResourceRegistry();

		public static IReadableResourceRegistry Instance
		{
			get { return _instance; }
		}

		private NullResourceRegistry()
		{
		}

		public IEnumerable<string> GetIncludes()
		{
			yield break;
		}

		public IEnumerable<Action<TextWriter>> GetInlineBlocks()
		{
			yield break;
		}

		public bool TryResolvePath(string path, out string resolvedVirtualPath)
		{
			resolvedVirtualPath = path;
			return true;
		}

		public void IncludePath(string urlToInclude) {}
		public void RegisterInlineBlock(Action<TextWriter> block, object key) {}
		public bool IsInlineBlockRegistered(object key)
		{
			return false;
		}
	}
}