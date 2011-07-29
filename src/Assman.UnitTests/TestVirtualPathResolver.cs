using System;

using NUnit.Framework;

namespace Assman
{
	[TestFixture]
	public class TestVirtualPathResolver
	{
		private const string _appPath = "C:\\MyTestDir\\";
		private IPathResolver _resolver = new VirtualPathResolver(_appPath);

		[Test]
		public void MapsApplicationPath()
		{
			string mappedPath = _resolver.MapPath("~/SubDir/File.txt");

            Assert.That(mappedPath, Is.EqualTo(_appPath + "SubDir\\File.txt").IgnoreCase);
		}

		[Test]
		public void MapsAbsolutePath()
		{
			string mappedPath = _resolver.MapPath("/SubDir/File.txt");
            Assert.That(mappedPath, Is.EqualTo(_appPath + "SubDir\\File.txt").IgnoreCase);
		}
	}
}