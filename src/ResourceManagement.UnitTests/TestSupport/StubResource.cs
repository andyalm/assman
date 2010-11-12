using System;

namespace AlmWitt.Web.ResourceManagement.UnitTests.TestSupport
{
	internal class StubResource : IResource
	{
		private string _content;
		private string _name;
		private string _virtualPath;
		private DateTime _lastModified;

		public StubResource(string content)
		{
			_content = content;
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public string VirtualPath
		{
			get { return _virtualPath; }
			set { _virtualPath = value; }
		}

		public DateTime LastModified
		{
			get { return _lastModified; }
			set { _lastModified = value; }
		}

		public string GetContent()
		{
			return _content;
		}
	}
}
