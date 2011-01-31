using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using AlmWitt.Web.ResourceManagement.Configuration;

namespace AlmWitt.Web.ResourceManagement
{
    /// <summary>
    /// Represents a specific type of web resource that can be consolidated and included on a web page.
    /// </summary>
	public abstract class ResourceType
    {
		private static readonly ResourceType _clientScript = new ClientScriptResourceType();
		private static readonly ResourceType _css = new CssResourceType();

		/// <summary>
		/// Gets the client script resource type (i.e. javascript).
		/// </summary>
		public static ResourceType ClientScript
    	{
    		get { return _clientScript; }
    	}

		/// <summary>
		/// Gets the css resource type.
		/// </summary>
		public static ResourceType Css
    	{
    		get { return _css; }
    	}

		public static ResourceType FromPath(string path)
		{
			var extension = Path.GetExtension(path).ToLowerInvariant();
			if (ClientScript.DefaultFileExtension == extension)
				return ClientScript;
			if (Css.DefaultFileExtension == extension)
				return Css;

			throw new ArgumentException("The path with extension '" + extension + "' does not map to a known ResourceType.");
		}

		private readonly List<string> _fileExtensions = new List<string>();
		
		public void AddFileExtension(string fileExtension)
		{
			if(!_fileExtensions.Any(extension => extension.Equals(fileExtension, StringComparison.OrdinalIgnoreCase)))
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

        
		/// <summary>
		/// Gets the consolidated url of the given resource path using the given configuration.
		/// </summary>
		/// <param name="config"></param>
		/// <param name="resourcePath"></param>
		/// <returns></returns>
		public abstract string GetResourceUrl(ResourceManagementConfiguration config, string resourcePath);
    	
		/// <summary>
		/// Gets an instance of a <see cref="IResourceIncluder"/> for this resource type.
		/// </summary>
		/// <param name="includerFactory"></param>
		/// <returns></returns>
		public abstract IResourceIncluder GetIncluder(IResourceIncluderFactory includerFactory);
    }

	internal class ClientScriptResourceType : ResourceType
	{
		public override string ContentType
			{
				get { return "text/javascript"; }
			}

			public override string DefaultFileExtension
			{
				get { return ".js"; }
			}

		public override string GetResourceUrl(ResourceManagementConfiguration config, string resourcePath)
		{
			return config.GetScriptUrl(resourcePath);
		}

		public override IResourceIncluder GetIncluder(IResourceIncluderFactory includerFactory)
		{
			return includerFactory.CreateClientScriptIncluder();
		}
	}

	internal class CssResourceType : ResourceType
	{
		public override string ContentType
			{
				get { return "text/css"; }
			}

			public override string DefaultFileExtension
			{
				get { return ".css"; }
			}

		public override string GetResourceUrl(ResourceManagementConfiguration config, string resourcePath)
		{
			return config.GetStylesheetUrl(resourcePath);
		}

		public override IResourceIncluder GetIncluder(IResourceIncluderFactory includerFactory)
		{
			return includerFactory.CreateCssIncluder();
		}
	}
}
