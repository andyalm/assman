using System;
using System.Collections.Generic;
using System.IO;

namespace Assman.Registration
{
	public class GenericResourceRegistry : IReadableResourceRegistry
	{
		private readonly HashSet<string> _includes = new HashSet<string>(Comparers.VirtualPath);
		private readonly Dictionary<object, Action<TextWriter>> _inlineBlocks = new Dictionary<object, Action<TextWriter>>();

		public bool TryResolvePath(string resourcePath, out IEnumerable<string> resolvedResourcePaths)
		{
			resolvedResourcePaths = new[] {resourcePath};
			return true;
		}

		public IEnumerable<string> GetIncludes()
		{
			return _includes;
		}

		public IEnumerable<Action<TextWriter>> GetInlineBlocks()
		{
			return _inlineBlocks.Values;
		}

		public void Require(string resourcePath)
		{
			if (!_includes.Contains(resourcePath))
			{
				_includes.Add(resourcePath);
			}
		}

		public void RegisterInlineBlock(Action<TextWriter> block, object key)
		{
			if(key == null)
			{
				_inlineBlocks.Add(block, block);
			}
			else
			{
				if (!_inlineBlocks.ContainsKey(key))
				{
					_inlineBlocks.Add(key, block);
				}
			}
		}
		public bool IsInlineBlockRegistered(object key)
		{
			return _inlineBlocks.ContainsKey(key);
		}
	}
}