using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Assman
{
	/// <summary>
	/// Represents a specific type of web resource that can be consolidated and included on a web page.
	/// </summary>
	public abstract class ResourceType
	{
		private static readonly ResourceType _script = new ScriptResourceType();
		private static readonly ResourceType _stylesheet = new StylesheetResourceType();

		/// <summary>
		/// Gets the client script resource type (e.g. javascript).
		/// </summary>
		public static ResourceType Script
		{
			get { return _script; }
		}

		/// <summary>
		/// Gets the stylesheet resource type (e.g. css).
		/// </summary>
		public static ResourceType Stylesheet
		{
			get { return _stylesheet; }
		}

		public static ResourceType FromPath(string path)
		{
			var extension = Path.GetExtension(path).ToLowerInvariant();
			if (Script.DefaultFileExtension == extension)
				return Script;
			if (Stylesheet.DefaultFileExtension == extension)
				return Stylesheet;

			throw new ArgumentException("The path with extension '" + extension + "' does not map to a known ResourceType.");
		}

		private readonly List<string> _fileExtensions = new List<string>();

		public void AddFileExtension(string fileExtension)
		{
			if (!_fileExtensions.Any(extension => extension.Equals(fileExtension, StringComparison.OrdinalIgnoreCase)))
				_fileExtensions.Add(fileExtension);
		}

		public IEnumerable<string> FileExtensions
		{
			get
			{
				yield return DefaultFileExtension;
				foreach (var fileExtension in _fileExtensions)
				{
					yield return fileExtension;
				}
			}
		}

		public string Separator
		{
			get { return Environment.NewLine; }
		}

		public abstract string ContentType { get; }
		public abstract string DefaultFileExtension { get; }
	}

	internal class ScriptResourceType : ResourceType
	{
		public override string ContentType
		{
			get { return "text/javascript"; }
		}

		public override string DefaultFileExtension
		{
			get { return ".js"; }
		}
	}

	internal class StylesheetResourceType : ResourceType
	{
		public override string ContentType
		{
			get { return "text/css"; }
		}

		public override string DefaultFileExtension
		{
			get { return ".css"; }
		}
	}
}
