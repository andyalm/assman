using System.Collections.Generic;

using AlmWitt.Web.ResourceManagement.IO;
using AlmWitt.Web.ResourceManagement.TestSupport;

using Moq;

using NUnit.Framework;

namespace AlmWitt.Web.ResourceManagement.PreConsolidation
{
	[TestFixture]
	public class TestCompiledFilePersister
	{
		private CompiledFilePersister _persister;
		private StubFileAccess _fileAccess;

		[SetUp]
		public void SetupContext()
		{
			_fileAccess = new StubFileAccess();
			
			_persister = new CompiledFilePersister(_fileAccess);
		}

		[Test]
		public void WhenReportIsSaved_ItCanBeRetrievedWithIdenticalData()
		{
			var report = new PreConsolidationReport
			{
				ClientScriptGroups = new List<PreConsolidatedResourceGroup>
				{
					new PreConsolidatedResourceGroup
					{
						ConsolidatedUrl = "~/scripts/consolidated/common.js",
						Resources = new List<PreConsolidatedResourcePiece>
						{
							new PreConsolidatedResourcePiece
							{
								Path = "~/scripts/jquery.js"
							},
							new PreConsolidatedResourcePiece
							{
								Path = "~/scripts/myscript1.js",
								Dependencies = new List<string>
								{
									"~/scripts/jquery.js"
								}
							}
						}
					},
					new PreConsolidatedResourceGroup
					{
						ConsolidatedUrl = "~/scripts/consolidated/search.js",
						Resources = new List<PreConsolidatedResourcePiece>
						{
							new PreConsolidatedResourcePiece
							{
								Path = "~/Views/Search/Index.js",
								Dependencies = new List<string>
								{
									"~/scripts/jquery.js",
									"~/scripts/myscript1.js"
								}
							}
						}
					}
				},
				CssGroups = new List<PreConsolidatedResourceGroup>
				{
					new PreConsolidatedResourceGroup
					{
						ConsolidatedUrl = "~/Content/consolidated.css",
						Resources = new List<PreConsolidatedResourcePiece>
						{
							new PreConsolidatedResourcePiece
							{
								Path = "~/Content/Site.css"
							},
							new PreConsolidatedResourcePiece
							{
								Path = "~/Views/Search/Search.css"
							}
						}
					}
				},
				Version = "321"
			};

			_persister.SavePreConsolidationInfo(report);

			PreConsolidationReport deserializedReport;
			_persister.TryGetPreConsolidationInfo(out deserializedReport).ShouldBeTrue();

			ResourceGroupsShouldBeEqual(report.ClientScriptGroups, deserializedReport.ClientScriptGroups);
			ResourceGroupsShouldBeEqual(report.CssGroups, deserializedReport.CssGroups);
			report.Version.ShouldEqual(deserializedReport.Version);
		}

		private void ResourceGroupsShouldBeEqual(List<PreConsolidatedResourceGroup> groups1, List<PreConsolidatedResourceGroup> groups2)
		{
			groups1.CountShouldEqual(groups2.Count);
			for (int i = 0; i < groups1.Count; i++)
			{
				groups1[i].ConsolidatedUrl.ShouldEqual(groups2[i].ConsolidatedUrl);
				groups1[i].Resources.CountShouldEqual(groups2[i].Resources.Count);
				ResourcePiecesShouldBeEqual(groups1[i].Resources, groups2[i].Resources);
			}
		}

		private void ResourcePiecesShouldBeEqual(List<PreConsolidatedResourcePiece> pieces1,
		                                         List<PreConsolidatedResourcePiece> pieces2)
		{
			pieces1.CountShouldEqual(pieces2.Count);
			for (int i = 0; i < pieces1.Count; i++)
			{
				pieces1[i].Path.ShouldEqual(pieces2[i].Path);
				
				pieces1[i].Dependencies.ShouldContainAll(pieces2[i].Dependencies);
			}

		}
	}
}