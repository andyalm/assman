using System;
using System.IO;
using System.Reflection;

namespace Assman
{
	/// <summary>
	/// Represents a resource embedded in an assembly.
	/// </summary>
	public class EmbeddedResource : IResource
	{
		/// <summary>
		/// Creates the virtual path for an EmbeddedResource used for pattern matching in the resource management config file.
		/// </summary>
		/// <param name="assemblyName"></param>
		/// <param name="resourceName"></param>
		/// <returns></returns>
		public static string GetVirtualPath(string assemblyName, string resourceName)
		{
			return String.Format("assembly://{0}/{1}", assemblyName, resourceName);
		}

		public static bool IsVirtualPath(string path)
		{
			return path.StartsWith("assembly://");
		}

		private readonly Assembly _assembly;

		private readonly string _resourceName;

		private DateTime? _lastModified;

		/// <summary>
		/// Creates a new instance of an <see cref="EmbeddedResource"/>.
		/// </summary>
		/// <param name="assembly">The assembly the resource is embedded in.</param>
		/// <param name="resourceName">The full name of the resource (e.g. MyNamespace.MyResource.txt)</param>
		public EmbeddedResource(Assembly assembly, string resourceName)
		{
			_assembly = assembly;
			_resourceName = resourceName;
		}

		/// <summary>
		/// Gets the name of the resource.
		/// </summary>
		public string Name
		{
			get { return _resourceName; }
		}

		/// <summary>
		/// Gets the Virtual Path of the resource.
		/// </summary>
		public string VirtualPath
		{
			get { return GetVirtualPath(_assembly.GetName().Name, _resourceName); }
		}

		public string FileExtension
		{
			get { return Path.GetExtension(_resourceName); }
		}
		
		/// <summary>
		/// Gets the date that the resource was last modified.
		/// </summary>
		public DateTime LastModified
		{
			get
			{
				if(_lastModified == null)
				{
					_lastModified = _assembly.GetLastWriteTime();
				}

				return _lastModified.Value;
			}
		}

		/// <summary>
		/// Gets the content of the resource.
		/// </summary>
		/// <returns></returns>
		public string GetContent()
		{
			using(StreamReader reader = new StreamReader(_assembly.GetManifestResourceStream(_resourceName)))
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
			return VirtualPath;
		}
	}
}
