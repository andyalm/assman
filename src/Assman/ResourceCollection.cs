using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Assman
{
	/// <summary>
	/// Represents a collection of <see cref="IResource"/>s.
	/// </summary>
	public class ResourceCollection : Collection<IResource>
	{
		/// <summary>
		/// Returns whether the given object is equal to this object.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			return Equals(obj as ResourceCollection);
		}

		/// <summary>
		/// Returns whether the given <see cref="ResourceCollection"/> contains the same (or equivilent) resources
		/// as this instance.
		/// </summary>
		/// <param name="resourceCollection"></param>
		/// <returns></returns>
		public bool Equals(ResourceCollection resourceCollection)
		{
			if (this == resourceCollection) return true;
			if (resourceCollection == null) return false;

			if (this.Count != resourceCollection.Count)
				return false;

			return this.All(resourceCollection.Contains);
		}

		/// <summary>
		/// Returns a hashcode generated from the contained resources.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return this.Count + this.Sum(resource => resource.GetHashCode());
		}
	}
}
