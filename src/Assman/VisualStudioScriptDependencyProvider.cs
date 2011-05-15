using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Assman
{
	public class VisualStudioScriptDependencyProvider : IDependencyProvider
	{
		private static readonly Regex _referenceRegex = new Regex(@"///\s*(<reference .+ />)", RegexOptions.Compiled);
		private static readonly Regex _vsDocRegex = new Regex(@"-vsdoc(?=\.js)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		public static VisualStudioScriptDependencyProvider GetInstance()
		{
			return new VisualStudioScriptDependencyProvider();
		}
		
		internal VisualStudioScriptDependencyProvider()
		{
			
		}
		
		public IEnumerable<string> GetDependencies(IResource resource)
		{
			var matches = _referenceRegex.Matches(resource.GetContent());

			return (from match in matches.Cast<Match>()
					let virtualPath = ParseReferenceElement(match.Groups[1].Value, resource)
					where !String.IsNullOrEmpty(virtualPath)
			        select virtualPath).ToList();
		}

		private string ParseReferenceElement(string serializedReferenceElement, IResource resource)
		{
			XElement referenceElement;
			try
			{
				referenceElement = XElement.Parse(serializedReferenceElement);
			}
			catch (Exception)
			{
				return null;
			}
			var pathAttr = referenceElement.Attribute("path");
			if (pathAttr != null)
			{
                string path = RemoveVsDocIfPresent(pathAttr.Value);
			    path = ResolvePathToAppRelative(path, resource);

			    return path;
			}	

			var nameAttr = referenceElement.Attribute("name");
			if(nameAttr != null)
			{
				return CreateAssemblyVirtualPath(referenceElement, nameAttr.Value);
			}

			return null;
		}

	    private string ResolvePathToAppRelative(string unresolvedPath, IResource resource)
	    {
	        var result = unresolvedPath.ToAppPath(resource);
	        return result;
	    }

	    private string RemoveVsDocIfPresent(string path)
		{
			return _vsDocRegex.Replace(path, String.Empty);
		}

		private string CreateAssemblyVirtualPath(XElement referenceElement, string resourceName)
		{
			string assemblyName = "System.Web.Extensions";
			var assemblyAttr = referenceElement.Attribute("assembly");
			if (assemblyAttr != null)
				assemblyName = assemblyAttr.Value;

			return String.Format("assembly://{0}/{1}", assemblyName, resourceName);
		}
	}
}