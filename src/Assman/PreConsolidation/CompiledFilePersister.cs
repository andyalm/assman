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
			var fileAccess = new FileAccessWrapper(Path.Combine(rootWebDirectory, "bin\\Assman.compiled"));
			return new CompiledFilePersister(fileAccess);
		}
		
		private readonly IFileAccess _fileAccess;

		internal CompiledFilePersister(IFileAccess fileAccess)
		{
			_fileAccess = fileAccess;
		}

		public bool TryGetPreConsolidationInfo(out PreConsolidationReport preConsolidationReport)
		{
			if (!_fileAccess.Exists())
			{
				preConsolidationReport = null;
				return false;
			}

			preConsolidationReport = new PreConsolidationReport();
			XDocument document;
			using (var reader = _fileAccess.OpenReader())
			{
				document = XDocument.Load(reader);
			}
			
			preConsolidationReport.Version = (string) document.Root.Attribute("version");
			preConsolidationReport.ScriptGroups = CollectResourceGroups(document.Root.Element("scriptGroups")).ToList();
			preConsolidationReport.StyleGroups = CollectResourceGroups(document.Root.Element("styleGroups")).ToList();
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
			using(var writer = XmlWriter.Create(_fileAccess.OpenWriter(), xmlWriterSettings))
			{
				using(writer.Document())
				{
					using (writer.Element("preConsolidationReport", version => preConsolidationReport.Version))
					{
						WriteResourceGroups(writer, "scriptGroups", preConsolidationReport.ScriptGroups);
						WriteResourceGroups(writer, "styleGroups", preConsolidationReport.StyleGroups);
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

		private void WriteResourceGroups(XmlWriter writer, string collectionElementName,
		                                 IEnumerable<PreConsolidatedResourceGroup> groups)
		{
			using (writer.Element(collectionElementName))
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