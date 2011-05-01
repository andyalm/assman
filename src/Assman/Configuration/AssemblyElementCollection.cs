using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;

namespace Assman.Configuration
{
	/// <summary>
	/// Represents a collection of <see cref="AssemblyElement"/>s.
	/// </summary>
	public class AssemblyElementCollection : ConfigurationElementCollection
	{
		/// <summary>
		/// Adds an <see cref="AssemblyElement"/> with the given name to the collection.
		/// </summary>
		/// <param name="assemblyName"></param>
		public void Add(string assemblyName)
		{
			BaseAdd(new AssemblyElement(assemblyName));
		}

		/// <summary>
		/// Returns whether the collection contains a <see cref="AssemblyElement"/> with the given name.
		/// </summary>
		/// <param name="assemblyName"></param>
		/// <returns></returns>
		public bool Contains(string assemblyName)
		{
			return this.BaseGet(assemblyName) != null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected override ConfigurationElement CreateNewElement()
		{
			return new AssemblyElement();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((AssemblyElement)element).Name;
		}

		/// <summary>
		/// Gets the <see cref="Assembly"/>s that the collection is referring to.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Assembly> GetAssemblies()
		{
			foreach (AssemblyElement element in this)
			{
				yield return element.GetAssembly();
			}
		}
	}
}
