using System;
using System.Collections.Generic;

using Assman.TestSupport;

using NUnit.Framework;

namespace Assman.PreConsolidation
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
			
			_persister = new CompiledFilePersister(_fileAccess, "assman.compiled");
		}

		[Test]
		public void WhenReportIsSaved_ItCanBeRetrievedWithIdenticalData()
		{
			var report = new PreConsolidationReport
			{
				Scripts = new PreConsolidatedResourceReport
				{
					Groups = new List<PreConsolidatedResourceGroup>
					{
						new PreConsolidatedResourceGroup
						{
							ConsolidatedUrl = "~/scripts/consolidated/common.js",
							Resources = new List<string>
							{
								"~/scripts/jquery.js",
								"~/scripts/myscript1.js"
							}
						},
						new PreConsolidatedResourceGroup
						{
							ConsolidatedUrl = "~/scripts/consolidated/search.js",
							Resources = new List<string>
							{
								"~/Views/Search/Index.js"
							}
						}
					}
				},
				Stylesheets = new PreConsolidatedResourceReport
				{
					Groups = new List<PreConsolidatedResourceGroup>
					{
						new PreConsolidatedResourceGroup
						{
							ConsolidatedUrl = "~/Content/consolidated.css",
							Resources = new List<string>
							{
								"~/Content/Site.css",
								"~/Views/Search/Search.css"
							}
						}
					}
				},
				Dependencies = new List<PreConsolidatedResourceDependencies>
				{
					new PreConsolidatedResourceDependencies
					{
						ResourcePath = "~/scripts/myscript1.js",
						Dependencies = new List<string>
						{
							"~/scripts/jquery.js"
						}
					},
					new PreConsolidatedResourceDependencies
					{
						ResourcePath = "~/Views/Search/Index.js",
						Dependencies = new List<string>
						{
							"~/scripts/jquery.js",
							"~/scripts/myscript1.js"
						}
					}
				},
				Version = "321"
			};

			_persister.SavePreConsolidationInfo(report);

			PreConsolidationReport deserializedReport;
			_persister.TryGetPreConsolidationInfo(out deserializedReport).ShouldBeTrue();

			ResourceGroupsShouldBeEqual(report.Scripts.Groups, deserializedReport.Scripts.Groups);
			ResourceGroupsShouldBeEqual(report.Stylesheets.Groups, deserializedReport.Stylesheets.Groups);
			ResourceDependenciesShouldBeEqual(report.Dependencies, deserializedReport.Dependencies);
			report.Version.ShouldEqual(deserializedReport.Version);
		}

		private void ResourceGroupsShouldBeEqual(List<PreConsolidatedResourceGroup> groups1, List<PreConsolidatedResourceGroup> groups2)
		{
			groups1.CountShouldEqual(groups2.Count);
			for (int i = 0; i < groups1.Count; i++)
			{
				groups1[i].ConsolidatedUrl.ShouldEqual(groups2[i].ConsolidatedUrl);
				groups1[i].Resources.CountShouldEqual(groups2[i].Resources.Count);
				groups1[i].Resources.ShouldContainAll(groups2[i].Resources);
			}
		}

		private void ResourceDependenciesShouldBeEqual(List<PreConsolidatedResourceDependencies> dependencies1, List<PreConsolidatedResourceDependencies> dependencies2)
		{
			dependencies1.CountShouldEqual(dependencies2.Count);
			for (int i = 0; i < dependencies1.Count; i++)
			{
				dependencies1[i].ResourcePath.ShouldEqual(dependencies2[i].ResourcePath);
				dependencies1[i].Dependencies.CountShouldEqual(dependencies2[i].Dependencies.Count);
				dependencies1[i].Dependencies.ShouldContainAll(dependencies2[i].Dependencies);
			}
		}
	}
}