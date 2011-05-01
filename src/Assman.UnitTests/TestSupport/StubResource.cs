using System;
using System.IO;

namespace Assman.TestSupport
{
	public class StubResource : IResource
	{
		public static StubResource WithContent(string content)
		{
			return new StubResource(content);
		}

		public static StubResource WithPath(string virtualPath)
		{
			return new StubResource("")
			{
				VirtualPath = virtualPath
			};
		}

		private readonly string _content;

		private string _virtualPath;

		private DateTime _lastModified = DateTime.Now;

		public StubResource(string content)
		{
			_content = content;
		}

		public string Name
		{
			get
			{
				if (_virtualPath == null)
					return null;

				if (!_virtualPath.Contains("/"))
					return _virtualPath;

				return _virtualPath.Substring(_virtualPath.LastIndexOf("/") + 1);
			}
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

		public string FileExtension
		{
			get { return Path.GetExtension(VirtualPath); }
		}

		public string GetContent()
		{
			return _content;
		}

        public override string ToString()
        {
            return VirtualPath;
        }
	}
}
