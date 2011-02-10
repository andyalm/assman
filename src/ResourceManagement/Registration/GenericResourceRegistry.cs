using System;
using System.Collections.Generic;
using System.IO;

namespace AlmWitt.Web.ResourceManagement
{
	public class GenericResourceRegistry : IReadableResourceRegistry, IScriptRegistry, IStyleRegistry
	{
		private readonly HashSet<string> _includes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		private readonly Dictionary<object, Action<TextWriter>> _inlineBlocks = new Dictionary<object, Action<TextWriter>>();

		public IEnumerable<string> GetIncludes()
		{
			return _includes;
		}

		public IEnumerable<Action<TextWriter>> GetInlineBlocks()
		{
			return _inlineBlocks.Values;
		}

		public void IncludeUrl(string urlToInclude)
		{
			if (!_includes.Contains(urlToInclude))
			{
				_includes.Add(urlToInclude);
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

		public virtual string GetEmbeddedResourceUrl(string assemblyName, string resourceName)
		{
			throw new NotSupportedException(@"Directly including embedded resources is currently only supported when using the WebForms View Engine.
You should still be able to consolidate embedded resources into consolidated groups without WebForms however.");
		}
	}
}