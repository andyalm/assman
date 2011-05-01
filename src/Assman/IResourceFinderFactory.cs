using System;

namespace Assman
{
	public interface IResourceFinderFactory
	{
		IResourceFinder CreateFinder();
	}
}