using System;
using System.Collections.Generic;
using System.IO;

namespace AlmWitt.Web.ResourceManagement
{
	/// <summary>
	/// An implementation of <see cref="IResourceRegistry"/> that doesn't do anything. (null object pattern)
	/// </summary>
	public class NullResourceRegistry : IReadableResourceRegistry, IScriptRegistry, IStyleRegistry
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

		public string GetEmbeddedResourceUrl(string assemblyName, string resourceName)
		{
			return null;
		}

		public void IncludeUrl(string urlToInclude) {}
		public void RegisterInlineBlock(Action<TextWriter> block, object key) {}
		public bool IsInlineBlockRegistered(object key)
		{
			return false;
		}
	}
}