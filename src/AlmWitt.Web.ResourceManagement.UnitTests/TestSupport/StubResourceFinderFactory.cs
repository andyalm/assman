using System;

namespace AlmWitt.Web.ResourceManagement.UnitTests.TestSupport
{
    public class StubResourceFinderFactory : IResourceFinderFactory
    {
        public IResourceFinder CreateFinder()
        {
            return new StubResourceFinder();
        }
    }
}