using System;

namespace AlmWitt.Web.ResourceManagement.TestSupport
{
    public class StubResourceFinderFactory : IResourceFinderFactory
    {
        public IResourceFinder CreateFinder()
        {
            return new StubResourceFinder();
        }
    }
}