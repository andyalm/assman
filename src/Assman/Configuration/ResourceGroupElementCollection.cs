
using System;
using System.Collections.Generic;
using System.Configuration;

namespace Assman.Configuration
{
	/// <summary>
	/// Represents a collection of <see cref="ResourceGroupElement"/>'s.
	/// </summary>
	public abstract class ResourceGroupElementCollection : ConfigurationElementCollection, IEnumerable<ResourceGroupElement>
	{
		/// <summary>
		/// Gets a <see cref="ResourceGroupElement"/> by index number.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public ResourceGroupElement this[int index]
		{
			get
			{
				return (ResourceGroupElement) BaseGet(index);
			}
		}

		/// <summary>
		/// Adds the given <see cref="ResourceGroupElement"/> to the collection.
		/// </summary>
		/// <param name="element"></param>
		public void Add(ResourceGroupElement element)
		{
			BaseAdd(element);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected override sealed ConfigurationElement CreateNewElement()
		{
		    return CreateGroupElement();
		}

		protected abstract ResourceGroupElement CreateGroupElement();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((ResourceGroupElement) element).ConsolidatedUrl;
		}

		#region IEnumerable<ResourceGroupElement> implimentation

		IEnumerator<ResourceGroupElement> IEnumerable<ResourceGroupElement>.GetEnumerator()
		{
			foreach (ResourceGroupElement element in this)
			{
				yield return element;
			}
		}

		#endregion
	}

	internal delegate void ResourceGroupElementProcessor<in TGroupElement>(TGroupElement groupElement, IResourceFilter excludeFilter)
		where TGroupElement : ResourceGroupElement, new();
}
