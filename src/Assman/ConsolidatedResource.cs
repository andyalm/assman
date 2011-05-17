using System;
using System.Collections.Generic;
using System.IO;

namespace Assman
{
	/// <summary>
	/// Represents a collection of resources that have been consolidated
	/// into one resource.
	/// </summary>
	public class ConsolidatedResource
	{
		private readonly ResourceCollection _resources;
		private readonly DateTime _lastModified;
		private readonly MemoryStream _contentStream;

		internal ConsolidatedResource(ResourceCollection resources, MemoryStream consolidatedContent)
		{
			_resources = resources;
			_lastModified = resources.LastModified();
			_contentStream = consolidatedContent;
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
			//ensure the destination directory exists
			string directory = Path.GetDirectoryName(path);
			Directory.CreateDirectory(directory);
			
			using(Stream outputStream = new FileStream(path, FileMode.Create))
			{
				WriteTo(outputStream);
			}
		}

		public void WriteTo(Stream outputStream)
		{
			if(ContentStream.Length > 0)
			{
				ContentStream.WriteTo(outputStream);	
			}
		}

		public void WriteSummaryHeader(Stream outputStream)
		{
			var writer = new StreamWriter(outputStream);
			try
			{
				writer.Write("/*");
				writer.WriteLine("This file consists of content from: ");
				foreach (var resource in Resources)
				{
					writer.WriteLine("\t" + resource.VirtualPath);
				}
			}
			finally
			{
				writer.WriteLine("*/");
				writer.WriteLine();
				writer.Flush();
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
