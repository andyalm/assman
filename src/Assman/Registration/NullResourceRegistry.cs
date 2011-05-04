using System;
using System.Collections.Generic;
using System.IO;

namespace Assman.Registration
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

		public bool TryResolvePath(string resourcePath, out IEnumerable<string> resolvedResourcePaths)
		{
			resolvedResourcePaths = new [] {resourcePath};
			return true;
		}

		public void Require(string resourcePath) {}
		public void RegisterInlineBlock(Action<TextWriter> block, object key) {}
		public bool IsInlineBlockRegistered(object key)
		{
			return false;
		}
	}
}