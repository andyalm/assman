using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

using AlmWitt.Web.ResourceManagement.IO;
using AlmWitt.Web.ResourceManagement.Xml;

namespace AlmWitt.Web.ResourceManagement.PreConsolidation
{
	public class CompiledFilePersister : IPreConsolidationReportPersister
	{
		public static CompiledFilePersister ForWebDirectory(string rootWebDirectory)
		{
			var fileAccess = new FileAccessWrapper(Path.Combine(rootWebDirectory, "bin\\ResourceManagement.compiled"));
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
			var document = XDocument.Load(_fileAccess.OpenReader());

			preConsolidationReport.ClientScriptGroups = CollectResourceGroups(document.Root.Element("scripts")).ToList();
			preConsolidationReport.CssGroups = CollectResourceGroups(document.Root.Element("stylesheets")).ToList();

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
					using (writer.Element("preConsolidationReport"))
					{
						WriteResourceGroups(writer, "scripts", preConsolidationReport.ClientScriptGroups);
						WriteResourceGroups(writer, "stylesheets", preConsolidationReport.CssGroups);
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
			       	Resources = groupElement.Elements("resource").Select(r => new PreConsolidatedResourcePiece
			       	{
			       		Path = (string) r.Attribute("path"),
			       		Dependencies = r.Elements("dependency").Select(d => (string) d.Attribute("path")).ToList()
			       	}).ToList()
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
							using(writer.Element("resource", path => resource.Path))
							{
								foreach (var dependency in resource.Dependencies)
								{
									using(writer.Element("dependency", path => dependency)) {}
								}
							}
						}
					}
				}
			}
		}
	}
}