using System;

namespace AlmWitt.Web.ResourceManagement.UnitTests.TestObjects
{
    public class StubResourceFinderFactory : IResourceFinderFactory
    {
        public IResourceFinder CreateFinder()
        {
            return new StubResourceFinder();
        }
    }
}