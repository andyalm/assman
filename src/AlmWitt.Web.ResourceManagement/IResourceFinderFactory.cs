using System;

namespace AlmWitt.Web.ResourceManagement
{
	public interface IResourceFinderFactory
	{
		IResourceFinder CreateFinder();
	}
}