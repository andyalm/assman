using NUnit.Framework;

using Assman.TestSupport;

namespace Assman
{
    [TestFixture]
    public class TestVirtualPathExtensions
    {
        [Test]
        public void RelativePathCanBeResolvedToAppRelative()
        {
            "../../MyScript.js".ToAppRelativePath("~/Views/Controller/Folder/MyFile.js").ShouldEqual("~/Views/MyScript.js");
        }

        [Test]
        public void AbsolutePathCanBeResolvedToAppRelative()
        {
            "/Scripts/MyScript.js".ToAppRelativePath("~/Views/Controller/Folder/MyFile.js").ShouldEqual("~/Scripts/MyScript.js");
        }

        [Test]
        public void AbsoluteUriCanBeResolvedToAppRelative()
        {
            "http://mywebsite.com/Scripts/MyScript.js".ToAppRelativePath("~/Views/Controller/Folder/MyFile.js").ShouldEqual("http://mywebsite.com/Scripts/MyScript.js");
        }

        [Test]
        public void AppRelativePathCanBeResolvedToAppRelative()
        {
            "~/Scripts/MyScript.js".ToAppRelativePath("~/Views/Controller/Folder/MyFile.js").ShouldEqual("~/Scripts/MyScript.js");
        }

        [Test]
        public void AppRelativePathCanBeResolvedToAbsolutePath()
        {
            "~/Scripts/MyScript.js".ToAbsolutePath().ShouldEqual("/Scripts/MyScript.js");
        }

        [Test]
        public void AbsolutePathCanBeResolvedToAbsolutePath()
        {
            "/Scripts/MyScript.js".ToAbsolutePath().ShouldEqual("/Scripts/MyScript.js");
        }

        [Test]
        public void AbsoluteUriCanBeResolvedToAbsolutePath()
        {
            "http://mywebsite.com/Scripts/MyScript.js".ToAbsolutePath().ShouldEqual("http://mywebsite.com/Scripts/MyScript.js");
        }
    }
}