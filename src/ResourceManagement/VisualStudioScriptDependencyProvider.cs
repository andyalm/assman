using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace AlmWitt.Web.ResourceManagement
{
	public class VisualStudioScriptDependencyProvider : IDependencyProvider
	{
		private static readonly Regex _referenceRegex = new Regex(@"///\s*(<reference .+ />)", RegexOptions.Compiled);

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
					let virtualPath = ParseReferenceElement(match.Groups[1].Value)
					where !String.IsNullOrEmpty(virtualPath)
			        select virtualPath).ToList();
		}

		private string ParseReferenceElement(string serializedReferenceElement)
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
				return pathAttr.Value;

			var nameAttr = referenceElement.Attribute("name");
			if(nameAttr != null)
			{
				return CreateAssemblyVirtualPath(referenceElement, nameAttr.Value);
			}

			return null;
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