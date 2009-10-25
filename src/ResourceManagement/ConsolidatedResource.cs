using System;
using System.IO;
using AlmWitt.Web.ResourceManagement.ContentFiltering;

namespace AlmWitt.Web.ResourceManagement
{
	/// <summary>
	/// Represents a collection of resources that have been consolidated
	/// into one resource.
	/// </summary>
	public class ConsolidatedResource
	{
		private ResourceCollection _resources;
		private DateTime _lastModified;
		private MemoryStream _contentStream;

		private ConsolidatedResource(ResourceCollection resources, IContentFilter filter, string separator)
		{
			_resources = resources;
			_lastModified = resources.LastModified;
			ConsolidateResources(filter, separator);
		}

		/// <summary>
		/// Returns a consolidated resource from the given resources.
		/// </summary>
		/// <param name="resources"></param>
		/// <returns></returns>
		public static ConsolidatedResource FromResources(ResourceCollection resources)
		{
			return new ConsolidatedResource(resources, null, null);
		}

		/// <summary>
		/// Returns a consolidated resource from the given resources.
		/// </summary>
		/// <param name="resources"></param>
		/// <param name="filter"></param>
		/// <param name="separator"></param>
		/// <returns></returns>
		public static ConsolidatedResource FromResources(ResourceCollection resources, IContentFilter filter, string separator)
		{
			return new ConsolidatedResource(resources, filter, separator);
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
		public ResourceCollection Resources
		{
			get { return _resources; }
		}

		/// <summary>
		/// Gets a stream to the consolidated content.
		/// </summary>
		public MemoryStream ContentStream
		{
			get
			{
				return _contentStream;
			}
		}

		/// <summary>
		/// Writes the contents of the consolidated resource to a file.
		/// </summary>
		/// <param name="path">The full path of the file to be written to.</param>
		public void WriteToFile(string path)
		{
		    string directory = Path.GetDirectoryName(path);
		    //ensure the destination directory exists
            Directory.CreateDirectory(directory);
            
            using(Stream outputStream = new FileStream(path, FileMode.Create))
			{
				ContentStream.WriteTo(outputStream);
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

		private void ConsolidateResources(IContentFilter filter, string separator)
		{
			_contentStream = new MemoryStream();
			StreamWriter writer = new StreamWriter(_contentStream);
			_resources.Consolidate(writer, filter, separator);
			writer.Flush();
		}
	}
}