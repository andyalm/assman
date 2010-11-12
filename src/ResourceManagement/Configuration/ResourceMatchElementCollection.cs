using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace AlmWitt.Web.ResourceManagement.Configuration
{
	/// <summary>
	/// Represents a collection of patterns to match against resources.
	/// </summary>
	public class ResourceMatchElementCollection : ConfigurationElementCollection
	{
		/// <summary>
		/// Adds the given regex pattern to the collection.
		/// </summary>
		/// <param name="pattern">A regular expression pattern.</param>
		public void Add(string pattern)
		{
			var element = new ResourceMatchElement();
			element.Pattern = pattern;
			BaseAdd(element);
		}

		/// <summary>
		/// Gets whether the given resourcePath is matched by any of the
		/// patterns in the collection.
		/// </summary>
		/// <param name="resourcePath"></param>
		/// <returns></returns>
		public bool IsMatch(string resourcePath)
		{
			return this.Cast<ResourceMatchElement>().Any(element => element.IsMatch(resourcePath));
		}

		public IResourceMatch GetMatch(string resourcePath)
		{
			foreach (ResourceMatchElement element in this)
			{
				var match = element.GetMatch(resourcePath);
				if (match.IsMatch)
					return match;
			}

			return ResourceMatches.False(resourcePath);
		}

		/// <summary>
		/// Gets the zero-based index of the pattern that matches the given resourcePath.  If no match is found,
		/// -1 is returned.
		/// </summary>
		/// <param name="resourcePath"></param>
		/// <returns></returns>
		public int GetMatchIndex(string resourcePath)
		{
			int i = 0;
			foreach (ResourceMatchElement element in this)
			{
				if (element.IsMatch(resourcePath))
					return i;
				i += 1;
			}

			return -1;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected override ConfigurationElement CreateNewElement()
		{
			return new ResourceMatchElement();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((ResourceMatchElement) element).Pattern;
		}
	}
}
