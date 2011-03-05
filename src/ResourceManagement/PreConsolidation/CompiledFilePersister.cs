using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;

using AlmWitt.Web.ResourceManagement.IO;
using AlmWitt.Web.ResourceManagement.Xml;

namespace AlmWitt.Web.ResourceManagement.PreConsolidation
{
	public class CompiledFilePersister : IPreConsolidationReportPersister
	{
		private readonly IFileAccess _fileAccess;

		public CompiledFilePersister(string rootWebPath)
		{
			_fileAccess = new FileAccessWrapper(Path.Combine(rootWebPath, "bin\\ResourceManagement.compiled"));
		}

		internal CompiledFilePersister(IFileAccess fileAccess)
		{
			_fileAccess = fileAccess;
		}

		public bool TryGetPreConsolidationInfo(out PreConsolidationReport preConsolidationReport)
		{
			var document = XDocument.Load(_fileAccess.OpenReader());
			
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
						WriteResourceGroups(writer, "clientScripts", preConsolidationReport.ClientScriptGroups);
						WriteResourceGroups(writer, "cssFiles", preConsolidationReport.CssGroups);
					}
				}
			}
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
									using(writer.Element("dependency", src => dependency)) {}
								}
							}
						}
					}
				}
			}
		}
	}
}