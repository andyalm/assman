using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

using Assman.IO;
using Assman.Xml;

namespace Assman.PreCompilation
{
	public class CompiledFilePersister : IPreCompiledReportPersister
	{
		public static CompiledFilePersister ForWebDirectory(string rootWebDirectory)
		{
			var filePath = Path.Combine(rootWebDirectory, "bin\\Assman.compiled");
			var fileAccess = new FileAccessWrapper();
			return new CompiledFilePersister(fileAccess, filePath);
		}
		
		private readonly IFileAccess _fileAccess;
		private readonly string _filePath;

		internal CompiledFilePersister(IFileAccess fileAccess, string filePath)
		{
			_fileAccess = fileAccess;
			_filePath = filePath;
		}

		public bool TryGetPreConsolidationInfo(out PreCompilationReport preCompilationReport)
		{
			if (!_fileAccess.Exists(_filePath))
			{
				preCompilationReport = null;
				return false;
			}

			preCompilationReport = new PreCompilationReport();
			XDocument document;
			using (var reader = _fileAccess.OpenReader(_filePath))
			{
				document = XDocument.Load(reader);
			}
			
			preCompilationReport.Version = (string) document.Root.Attribute("version");
			preCompilationReport.Scripts = CollectResourceReport(document.Root.Element("scripts"));
			preCompilationReport.Stylesheets = CollectResourceReport(document.Root.Element("stylesheets"));
			preCompilationReport.Dependencies = CollectDependencies(document.Root.Element("dependencies")).ToList();

			return true;
		}


		public void SavePreConsolidationInfo(PreCompilationReport preCompilationReport)
		{
			var xmlWriterSettings = new XmlWriterSettings
			{
				CloseOutput = true,
				Indent = true
			};
			using(var writer = XmlWriter.Create(_fileAccess.OpenWriter(_filePath), xmlWriterSettings))
			{
				using(writer.Document())
				{
					using (writer.Element("PreCompilationReport", version => preCompilationReport.Version))
					{
						WriteResourceReport(writer, "scripts", preCompilationReport.Scripts);
						WriteResourceReport(writer, "stylesheets", preCompilationReport.Stylesheets);
						using(writer.Element("dependencies"))
						{
							foreach (var resourceWithDependency in preCompilationReport.Dependencies)
							{
								using (writer.Element("resource", path => resourceWithDependency.ResourcePath))
								{
									foreach (var dependency in resourceWithDependency.Dependencies)
									{
										using (writer.Element("dependency", path => dependency)) {}
									}
								}
							}
						}
					}
				}
			}
		}

		private PreCompiledResourceReport CollectResourceReport(XElement containerElement)
		{
			return new PreCompiledResourceReport
			{
				Groups = CollectResourceGroups(containerElement.Element("groups")).ToList(),
				SingleResources = CollectSingleResources(containerElement.Element("singleResources")).ToList()
			};
		}

		private IEnumerable<PreCompiledSingleResource> CollectSingleResources(XElement containerElement)
		{
			return from resourceElement in containerElement.Elements("resource")
				   select new PreCompiledSingleResource
				   {
					   OriginalPath = (string) resourceElement.Attribute("originalPath"),
					   CompiledPath = (string) resourceElement.Attribute("compiledPath")
				   };
		}

		private IEnumerable<PreCompiledResourceGroup> CollectResourceGroups(XElement containerElement)
		{
			return from groupElement in containerElement.Elements("group")
				   select new PreCompiledResourceGroup
				   {
					   ConsolidatedUrl = (string) groupElement.Attribute("consolidatedUrl"),
					   Resources = groupElement.Elements("resource").Select(r => (string) r.Attribute("path")).ToList()
				   };
		}

		private IEnumerable<PreCompiledResourceDependencies> CollectDependencies(XElement containerElement)
		{
			return from dependencyElement in containerElement.Elements("resource")
				   select new PreCompiledResourceDependencies
				   {
					   ResourcePath = (string) dependencyElement.Attribute("path"),
					   Dependencies =
						   dependencyElement.Elements("dependency").Select(d => (string) d.Attribute("path")).ToList()
				   };
		}

		private void WriteResourceReport(XmlWriter writer, string elementName, PreCompiledResourceReport resourceReport)
		{
			using (writer.Element(elementName))
			{
				WriteResourceGroups(writer, resourceReport.Groups);
			    WriteSingleResources(writer, resourceReport.SingleResources);
			}

		}

	    private void WriteSingleResources(XmlWriter writer, IEnumerable<PreCompiledSingleResource> resources)
	    {
	        using(writer.Element("singleResources"))
	        {
	            foreach (var resource in resources)
	            {
	                using(writer.Element("resource", originalPath => resource.OriginalPath, compiledPath => resource.CompiledPath)) {}
	            }
	        }
	    }

	    private void WriteResourceGroups(XmlWriter writer, IEnumerable<PreCompiledResourceGroup> groups)
		{
			using (writer.Element("groups"))
			{
				foreach (var @group in groups)
				{
					using (writer.Element("group", consolidatedUrl => @group.ConsolidatedUrl))
					{
						foreach (var resource in @group.Resources)
						{
							using (writer.Element("resource", path => resource)) {}
						}
					}
				}
			}
		}
	}
}