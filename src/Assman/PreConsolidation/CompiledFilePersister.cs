using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

using Assman.IO;
using Assman.Xml;

namespace Assman.PreConsolidation
{
	public class CompiledFilePersister : IPreConsolidationReportPersister
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

		public bool TryGetPreConsolidationInfo(out PreConsolidationReport preConsolidationReport)
		{
			if (!_fileAccess.Exists(_filePath))
			{
				preConsolidationReport = null;
				return false;
			}

			preConsolidationReport = new PreConsolidationReport();
			XDocument document;
			using (var reader = _fileAccess.OpenReader(_filePath))
			{
				document = XDocument.Load(reader);
			}
			
			preConsolidationReport.Version = (string) document.Root.Attribute("version");
			preConsolidationReport.Scripts = CollectResourceReport(document.Root.Element("scripts"));
			preConsolidationReport.Stylesheets = CollectResourceReport(document.Root.Element("stylesheets"));
			preConsolidationReport.Dependencies = CollectDependencies(document.Root.Element("dependencies")).ToList();

			return true;
		}


		public void SavePreConsolidationInfo(PreConsolidationReport preConsolidationReport)
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
					using (writer.Element("preConsolidationReport", version => preConsolidationReport.Version))
					{
						WriteResourceReport(writer, "scripts", preConsolidationReport.Scripts);
						WriteResourceReport(writer, "stylesheets", preConsolidationReport.Stylesheets);
						using(writer.Element("dependencies"))
						{
							foreach (var resourceWithDependency in preConsolidationReport.Dependencies)
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

		private PreConsolidatedResourceReport CollectResourceReport(XElement containerElement)
		{
			return new PreConsolidatedResourceReport
			{
				Groups = CollectResourceGroups(containerElement.Element("groups")).ToList()
			};
		}

		private IEnumerable<PreConsolidatedResourceGroup> CollectResourceGroups(XElement containerElement)
		{
			return from groupElement in containerElement.Elements("group")
				   select new PreConsolidatedResourceGroup
				   {
					ConsolidatedUrl = (string) groupElement.Attribute("consolidatedUrl"),
					Resources = groupElement.Elements("resource").Select(r => (string)r.Attribute("path")).ToList()
				   };
		}

		private IEnumerable<PreConsolidatedResourceDependencies> CollectDependencies(XElement containerElement)
		{
			return from dependencyElement in containerElement.Elements("resource")
				   select new PreConsolidatedResourceDependencies
				   {
					ResourcePath = (string) dependencyElement.Attribute("path"),
					Dependencies = dependencyElement.Elements("dependency").Select(d => (string) d.Attribute("path")).ToList()
				   };
		}

		private void WriteResourceReport(XmlWriter writer, string elementName, PreConsolidatedResourceReport resourceReport)
		{
			using(writer.Element(elementName))
			{
				WriteResourceGroups(writer, resourceReport.Groups);    
			}
			
		}

		private void WriteResourceGroups(XmlWriter writer, IEnumerable<PreConsolidatedResourceGroup> groups)
		{
			using (writer.Element("groups"))
			{
				foreach (var @group in groups)
				{
					using(writer.Element("group", consolidatedUrl => @group.ConsolidatedUrl))
					{
						foreach (var resource in @group.Resources)
						{
							using(writer.Element("resource", path => resource)) {}
						}
					}
				}
			}
		}
	}
}