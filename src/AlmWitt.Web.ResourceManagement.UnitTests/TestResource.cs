using System;
using System.IO;

using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace AlmWitt.Web.ResourceManagement.UnitTests
{
	[TestFixture]
	public class TestResource
	{
		[Test]
		public void VirtualPathTest()
		{
			string basePath = "C:\\MyTestDir";
			string filePath = "C:\\MyTestDir\\SubDir\\MyFile.js";
			IResource resource = new FileResource(filePath, basePath);

			Assert.That(resource.VirtualPath, Is.EqualTo("~/SubDir/MyFile.js").IgnoreCase);
		}
		
		[Test]
		public void LastModifiedMatchesFileLastWriteTime()
		{
			string filename = Path.GetTempFileName();
			File.Create(filename);
			IResource resource = new FileResource(filename, Path.GetDirectoryName(filename));

			Assert.That(resource.LastModified, Is.EqualTo(File.GetLastWriteTime(filename)));
		}
	}
}
