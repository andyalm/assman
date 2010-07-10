using System;
using System.IO;

namespace AlmWitt.Web.ResourceManagement
{
	/// <summary>
	/// Represents a file <see cref="IResource"/>.
	/// </summary>
	public class FileResource : IResource
	{
		private FileInfo _fileInfo;
		private string _filePath;
		private string _basePath;

		/// <summary>
		/// Creates a new instance of a <see cref="FileResource"/>.
		/// </summary>
		/// <param name="filePath">The physical path to the file.</param>
		/// <param name="basePath">The physical path to the directory where the virtual path starts from.</param>
		public FileResource(string filePath, string basePath)
		{
			_filePath = filePath;
			_basePath = AppendTrailingSlashIfNecessary(basePath);
		}

		/// <summary>
		/// Gets the file name.
		/// </summary>
		public string Name
		{
			get { return FileInfo.Name; }
		}

		/// <summary>
		/// Gets the virtual path constructed based on the base path.
		/// </summary>
		public string VirtualPath
		{
			get
			{
				string virtualPath = _filePath.ToLowerInvariant();
				virtualPath = virtualPath.Replace(_basePath.ToLowerInvariant(), "~/");
				virtualPath = virtualPath.Replace(@"\", "/");
				return virtualPath;
			}
		}

		/// <summary>
		/// Gets the time this file was last modified.
		/// </summary>
		public DateTime LastModified
		{
			get { return FileInfo.LastWriteTime; }
		}

		/// <summary>
		/// Gets the content of the file.
		/// </summary>
		/// <returns></returns>
		public virtual string GetContent()
		{
			using(StreamReader reader = new StreamReader(FileInfo.OpenRead()))
			{
				return reader.ReadToEnd();
			}
		}

		/// <summary>
		/// Returns a string representation of the <see cref="FileResource"/>.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Name;
		}

		protected FileInfo FileInfo
		{
			get
			{
				if(_fileInfo == null)
					_fileInfo = new FileInfo(_filePath);
				return _fileInfo;
			}
		}

		private static string AppendTrailingSlashIfNecessary(string path)
		{
			if (!path.EndsWith("\\"))
				path += "\\";
			return path;
		}
	}
}