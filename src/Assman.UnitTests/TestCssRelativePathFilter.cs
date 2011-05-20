using Assman.ContentFiltering;
using Assman.TestSupport;

using NUnit.Framework;

namespace Assman
{
    [TestFixture]
    public class TestCssRelativePathFilter
    {
        private CssRelativePathFilter _filter;
        private ContentFilterContext _context;

        [SetUp]
        public void SetupContext()
        {
            _filter = new CssRelativePathFilter();
            _context = new ContentFilterContext
            {
                ResourceVirtualPath = "~/Styles/AStylesheet.css",
                Group = new StubResourceGroup("~/Styles/Consolidated/OneStylesheet.css")
            };
        }

        [Test]
        public void RelativePathIsUpdatedToBeRelativeToConsolidatedUrl()
        {
            var content = @".myclass {
                background-image:url(../Images/MyImage.png);
            }";
            
            var filteredContent = _filter.FilterContent(content, _context);

            filteredContent.ShouldEqual(@".myclass {
                background-image:url(../../Images/MyImage.png);
            }");
        }

        [Test]
        public void AbsolutePathIsLeftAlone()
        {
            var content = @".myclass {
                background-image:url(/Images/MyImage.png);
            }";

            var filteredContent = _filter.FilterContent(content, _context);

            filteredContent.ShouldEqual(@".myclass {
                background-image:url(/Images/MyImage.png);
            }");
        }
    }
}