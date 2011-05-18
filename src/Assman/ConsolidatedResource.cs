using System;
using System.Collections.Generic;
using System.IO;

namespace Assman
{
	/// <summary>
	/// Represents a collection of resources that have been consolidated
	/// into one resource.
	/// </summary>
	internal class ConsolidatedResource : ICompiledResource
	{
		private readonly IResourceGroup _group;
	    private readonly ResourceCollection _resources;
		private readonly DateTime _lastModified;
		private readonly MemoryStream _contentStream;

		internal ConsolidatedResource(IResourceGroup group, ResourceCollection resources, MemoryStream consolidatedContent)
		{
		    _group = group;
		    _resources = resources;
			_lastModified = resources.LastModified();
			_contentStream = consolidatedContent;
		}


	    public string CompiledPath
	    {
	        get { return _group.ConsolidatedUrl; }
	    }

	    /// <summary>
		/// Gets the most recent last modified date of the resources the consolidated resources.
		/// </summary>
		public DateTime LastModified
		{
			get { return _lastModified; }
		}

		/// <summary>
		/// Gets the resources that were consolidated.
		/// </summary>
		public IEnumerable<IResource> Resources
		{
			get { return _resources; }
		}

		public void WriteTo(Stream outputStream)
		{
			if(_contentStream.Length > 0)
			{
				_contentStream.WriteTo(outputStream);	
			}
		}

		#region Equals and HashCode

		/// <summary>
		///
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if (this == obj) return true;
			ConsolidatedResource consolidatedResource = obj as ConsolidatedResource;
			if (consolidatedResource == null) return false;
			if (!Equals(_resources, consolidatedResource._resources)) return false;
			if (!Equals(_lastModified, consolidatedResource._lastModified)) return false;
			if (!Equals(_contentStream, consolidatedResource._contentStream)) return false;
			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			int result = _resources.GetHashCode();
			result = 29*result + _lastModified.GetHashCode();
			result = 29*result + (_contentStream != null ? _contentStream.GetHashCode() : 0);
			return result;
		}

		#endregion
	}
}
