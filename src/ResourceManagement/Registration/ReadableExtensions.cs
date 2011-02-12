using System;
using System.Collections.Generic;
using System.IO;

namespace AlmWitt.Web.ResourceManagement.Registration
{
	public static class ReadableExtensions
	{
		public static IReadableResourceRegistry AsReadable(this IResourceRegistry resourceRegistry)
		{
			if (resourceRegistry is IReadableResourceRegistry)
				return (IReadableResourceRegistry)resourceRegistry;

			return new MakeReadableResourceRegistry(resourceRegistry);
		}

		private class MakeReadableResourceRegistry : IReadableResourceRegistry
		{
			private readonly IResourceRegistry _inner;
			public MakeReadableResourceRegistry(IResourceRegistry inner)
			{
				_inner = inner;
			}

			public IEnumerable<string> GetIncludes()
			{
				return ReadableOrDefault().GetIncludes();
			}

			public IEnumerable<Action<TextWriter>> GetInlineBlocks()
			{
				return ReadableOrDefault().GetInlineBlocks();
			}

			public string GetEmbeddedResourceUrl(string assemblyName, string resourceName)
			{
				return _inner.GetEmbeddedResourceUrl(assemblyName, resourceName);
			}

			public void IncludeUrl(string urlToInclude)
			{
				_inner.IncludeUrl(urlToInclude);
			}

			public void RegisterInlineBlock(Action<TextWriter> block, object key)
			{
				_inner.RegisterInlineBlock(block, key);
			}

			public bool IsInlineBlockRegistered(object key)
			{
				return _inner.IsInlineBlockRegistered(key);
			}

			private IReadableResourceRegistry ReadableOrDefault()
			{
				return _inner is IReadableResourceRegistry ? (IReadableResourceRegistry)_inner : NullResourceRegistry.Instance;
			}
		}
	}
}