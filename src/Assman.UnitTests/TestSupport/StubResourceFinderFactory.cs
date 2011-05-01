using System;

namespace Assman.TestSupport
{
    public class StubResourceFinderFactory : IResourceFinderFactory
    {
        public IResourceFinder CreateFinder()
        {
            return new StubResourceFinder();
        }
    }
}