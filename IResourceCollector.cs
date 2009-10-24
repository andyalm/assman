using System;

namespace AlmWitt.Web.ResourceManagement
{
	internal interface IResourceCollector
	{
		ConsolidatedResource GetResource(IResourceFinder finder, string extension, IResourceFilter exclude);
	}
}